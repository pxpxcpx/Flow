using System.Diagnostics;
using Flow.Runtime.Utils;
using Flow.Runtime.ContextManager;
using Flow.Runtime.Models;
using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Results;

namespace Flow.Runtime;

public class Executor : IDisposable
{
    private INode? _current;

    private readonly ContextManager<Guid> _contextManager;

    private readonly Queue<INode>? _pendingNodes;

    public Script Script { get; set; }

    public ProcessorStatus Status { get; private set; }

    public Result? Result { get; private set; }

    public Executor(Script script, ContextManager<Guid>? contextManager = null)
    {
        _pendingNodes = new Queue<INode>();
        _contextManager = contextManager ?? new ContextManager<Guid>();
        Script = script ?? throw new ArgumentNullException(nameof(script));
    }

    #region Execution

    /// <summary>
    /// Execute the script.
    /// </summary>
    public void Execute()
    {
        if (Script.Entry is null) return;
        Execute(Script.Entry);
    }

    /// <summary>
    /// Invoke a node and its subsequent nodes iteratively.
    /// </summary>
    /// <param name="node"></param>
    private void Execute(INode node)
    {
        _current = node;
        _pendingNodes?.Enqueue(node);

        while (_pendingNodes is { Count: > 0 })
        {
            if (Status.HasFlag(ProcessorStatus.Paused) || Status.HasFlag(ProcessorStatus.Cancelled))
                return;

            var n = _pendingNodes.Dequeue();
            _current = n;
            ExecuteSingle(n);

            // If the node does not have a subsequent node (or the executor reaches to the end), return.
            var nextIds = MoveNext(node);
            if (nextIds is null || nextIds.Length == 0) return;

            // Get subsequent nodes and invoke them iteratively.
            foreach (var nextId in nextIds)
            {
                var next = Script.GetNode(nextId);
                _pendingNodes.Enqueue(next);
            }
        }
    }

    /// <summary>
    /// Pass the arguments and invoke a single node.
    /// </summary>
    /// <param name="node">Single node to be invoked.</param>
    private void ExecuteSingle(INode node)
    {
        // If the node has unfilled values:
        if (node.GetUnfilledRequiredValues().Any())
        {
            if (!TryGetValueFromSource(node))
            {
                node.MarkAs(NodeStatus.Waiting);
                return;
            }
        }

        // If the node requires an instance.
        if (node is IInstanceRequired)
        {
            if (!TryPassContextToNode(node))
            {
                node.MarkAs(NodeStatus.Waiting);
                return;
            }
        }

        var succeed = true;
        node.Status |= NodeStatus.Running;

        // Attention:
        // If the same node implements both synchronous and asynchronous interfaces,
        // only the synchronous method will be executed.
        switch (node)
        {
            // If possible, execute this method on the node.
            case IExecutableNode en:
            {
                try
                {
                    en.Execute();
                }
                catch (Exception ex)
                {
                    OnError(ex);
                }

                break;
            }

            // Execute asynchronously if the node implements the IAsyncExecutableNode.
            case IAsyncExecutableNode aen:
            {
                aen.ExecuteAsync().Await(OnError);
                break;
            }

            default:
                return;
        }

        if (succeed)
        {
            node.Status |= NodeStatus.Completed;
            node.Status |= NodeStatus.Ready;
        }

        if (!PassResults(node))
            Result = new Result(false, false, null, $"Value transfer error occurred on node: {node.RuntimeId}");

        return;

        void OnError(Exception ex)
        {
            succeed = false;
            node.Status |= NodeStatus.Failed;
            var result = node.Result;
            Debug.WriteLine($"Exception detected: {ex.Message}, result: {result}");
        }
    }

    /// <summary>
    /// Used to actively attempt to invoke nodes
    /// marked as <see cref="NodeStatus.Waiting"/> after parameters have been passed.
    /// </summary>
    /// <param name="node"></param>
    private void TryEnqueueIfNodeIsWaiting(INode node)
    {
        if (node.Status != NodeStatus.Waiting)
            return;

        _pendingNodes?.Enqueue(node);
    }

    /// <summary>
    /// Move to next nodes.
    /// </summary>
    /// <param name="node">Current node</param>
    /// <returns></returns>
    private Guid[]? MoveNext(INode node)
    {
        if (node is IControlStatement csn)
        {
            var i = csn.ReturnIndex;
            return Script.GetRuntimeGuid(node) is not { } cid ? null : Script.GetNextProgressNodes(cid, i);
        }

        return Script.GetRuntimeGuid(node) is not { } id ? null : Script.GetNextProgressNodes(id);
    }

    #endregion

    #region Variable Utils

    /// <summary>
    /// Get results from a node, and pass them according to connections. (Actively pass values)
    /// </summary>
    /// <param name="node">The node has completed processing and is ready to transmit the results to the next node</param>
    /// <returns></returns>
    private bool PassResults(INode node)
    {
        if (Script.GetRuntimeGuid(node) is not { } rt) return false;
        if (Script.GetVariableTarget(rt) is not { } targets) return false;

        var succeed = true;
        foreach (var p in targets)
        {
            var targetNode = Script.GetNode(p.NodeId);
            var value = node.GetOutput(p.Index);
            if (!targetNode.Assign(p.Index, value))
            {
                succeed = false;
                continue;
            }

            TryEnqueueIfNodeIsWaiting(targetNode);
        }

        return succeed;
    }

    /// <summary>
    /// Get results from related value sources. (Passive value retrieval)
    /// </summary>
    /// <param name="node">Node that requires passing values</param>
    /// <returns></returns>
    private bool TryGetValueFromSource(INode node)
    {
        if (Script.GetRuntimeGuid(node) is not { } rt) return false;
        if (Script.GetSourceVariableConnections(rt) is not { } source) return false;

        var succeed = true;
        foreach (var p in source)
        {
            var sn = Script.GetNode(p.From.NodeId);
            object? value;
            
            // If the value source is a value generator
            if (sn is IValueGeneratorNode gen)
            {
                // then execute it because it's passive.
                gen.Execute();
                value = gen.Outputs;
            }
            else
            {
                value = sn.GetOutput(p.Source.Index);
            }

            if (node.Assign(p.Target.Index, value))
                succeed = false;
        }

        return succeed;
    }

    /// <summary>
    /// Try to get a context from the ContextManager, and pass it to the node.
    /// <remarks>The node must implements <see cref="IInstanceRequired"/>.</remarks>
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private bool TryPassContextToNode(INode node)
    {
        if (Script.GetRuntimeGuid(node) is not { } rt) return false;
        if (Script.GetRelatedContext(rt) is not { } rc) return false;

        try
        {
            ((IInstanceRequired)node).Instance = _contextManager.TryFindContextObject(rc);
            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Process Control

    private void Pause()
    {
        Status |= ProcessorStatus.Paused;
    }

    private void Resume()
    {
        Status |= ProcessorStatus.Running;
        if (_current is null)
        {
            Execute();
            return;
        }

        Execute(_current);
    }

    private void Stop()
    {
        Status |= ProcessorStatus.Cancelled;
        Dispose();
    }

    #endregion

    public void Dispose()
    {
        _contextManager.Dispose();
        Script.Dispose();
        GC.SuppressFinalize(this);
    }
}