using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Flow.Runtime.Abstractions;
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

    private Function _function;

    [NotNull] 
    private Queue<INode>? _currentPendingNodes;

    private readonly Stack<KeyValuePair<Function, Queue<INode>>> _backupStack;

    // TODO: Context manager?How it works???
    private readonly ContextManager<Guid> _contextManager;

    public ProcessorStatus Status { get; private set; }

    public Result? Result { get; private set; }

    public Executor(Function function, ContextManager<Guid>? contextManager = null)
    {
        _currentPendingNodes = new Queue<INode>();
        _contextManager = contextManager ?? new ContextManager<Guid>();
        _function = function ?? throw new ArgumentNullException(nameof(function));

        _backupStack = new Stack<KeyValuePair<Function, Queue<INode>>>();
        _backupStack.Push(new KeyValuePair<Function, Queue<INode>>(function, new Queue<INode>()));
    }

    private Executor(IFunctionNode functionNode)
    {
        _currentPendingNodes = new Queue<INode>();
    }

    #region Execution

    /// <summary>
    /// Execute the script.
    /// </summary>
    public void Execute()
    {
        if (_function.Entry is null) return;
        Execute(_function.Entry);
    }

    /// <summary>
    /// Invoke a node and its subsequent nodes iteratively.
    /// </summary>
    /// <param name="node"></param>
    private void Execute(INode node)
    {
        _current = node;
        _currentPendingNodes.Enqueue(node);

        while (_currentPendingNodes is { Count: > 0 })
        {
            if (Status.HasFlag(ProcessorStatus.Paused) || Status.HasFlag(ProcessorStatus.Cancelled))
                return;

            var n = _currentPendingNodes.Dequeue();
            ExecuteSingle(n);
            _current = n;

            // If the node does not have a subsequent node (or the executor reaches to the end), return.
            // TODO: Pop the function stack to return, dequeue from here.
            var nextIds = MoveNext(node);
            if (nextIds is null || nextIds.Length == 0)
            {
                if (_function is INode)
                {
                    ReturnToPreviousFunction();
                    if (nextIds is null) continue;
                }
                else continue;
            }
            
            // Get subsequent nodes and invoke them iteratively.
            foreach (var nextId in nextIds)
            {
                var next = _function.GetNode(nextId);
                _currentPendingNodes.Enqueue(next);
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

            // Function node
            case IFunctionNode fn:
            {
                SwitchToSubFunction(fn);
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

        _currentPendingNodes?.Enqueue(node);
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
            return _function.GetRuntimeGuid(node) is not { } cid ? null : _function.GetNextProgressNodes(cid, i);
        }

        return _function.GetRuntimeGuid(node) is not { } id ? null : _function.GetNextProgressNodes(id);
    }

    /// <summary>
    /// Check the stack and return to origin function (caller).
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Throws when the function in stack is NOT a <see cref="FunctionNode"/>
    /// </exception>
    private void ReturnToPreviousFunction()
    {
        // _backupStack                             --return-> previous level
        if (!_backupStack.TryPop(out var pl))
            return;
        _function = pl.Key;
        
        // backup                                   --rejoin-> _currentPendingNodes
        _currentPendingNodes = Clone(pl.Value);
        
        // Pass results                             -> next
        if (pl.Key is not FunctionNode f)
            throw new InvalidOperationException();
        PassResults(f);
    }

    /// <summary>
    /// Switch to function inside a function.
    /// </summary>
    /// <param name="node"><see cref="FunctionNode"/></param>
    /// <exception cref="InvalidOperationException">
    /// Throws when <see cref="node"/> is NOT a <see cref="FunctionNode"/>
    /// </exception>
    private void SwitchToSubFunction(IFunctionNode node)
    {
        if (node is not FunctionNode f)
            throw new InvalidOperationException();

        // backup(NOT reference copy) _cpq          -> var backup
        var backup = Clone(_currentPendingNodes);
        var pair = new KeyValuePair<Function, Queue<INode>>(_function, backup);
        
        // function(with backup)                    -> _backupStack
        _backupStack.Push(pair);

        // result of the previous node (Re-assign)  -> function.Entry
        ResendInputs(f, f.Entry);
        
        // node.Entry                               -> _currentPendingQueue
        _function = f;
        _currentPendingNodes.Enqueue(f.Entry);
    }
    
    private static Queue<INode> Clone(Queue<INode> nodes)
    {
        var c = new Queue<INode>(nodes);
        while (nodes.TryDequeue(out var n))
        {
            c.Enqueue(n);
        }

        return c;
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
        if (_function.GetRuntimeGuid(node) is not { } rt) return false;
        if (_function.GetVariableTarget(rt) is not { } targets) return false;

        var succeed = true;
        foreach (var p in targets)
        {
            var targetNode = _function.GetNode(p.NodeId);
            if (node is IFunctionNode fn)
                targetNode = fn.Entry;

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
        if (_function.GetRuntimeGuid(node) is not { } rt) return false;
        if (_function.GetSourceVariableConnections(rt) is not { } source) return false;

        var succeed = true;
        foreach (var p in source)
        {
            var sn = _function.GetNode(p.Source.NodeId);
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
        if (_function.GetRuntimeGuid(node) is not { } rt) return false;
        if (_function.GetRelatedContext(rt) is not { } rc) return false;

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

    /// <summary>
    /// Resend one node's inputs to the other one.
    /// </summary>
    /// <remarks>Ensure two nodes' input metadata can be match.</remarks>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    private bool ResendInputs(INode source, INode target)
    {
        // Match two nodes' metadata
        if (source.InputVariableMetadata is null || target.InputVariableMetadata is null
           || source.Inputs is null || target.Inputs is null
           || source.InputVariableMetadata.Length != target.InputVariableMetadata.Length)
            return false;

        for (var i = 0; i < source.Inputs.Length; i++)
            target.Inputs[i] = source.Inputs[i];

        return true;
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
        _function.Dispose();
        GC.SuppressFinalize(this);
    }
}