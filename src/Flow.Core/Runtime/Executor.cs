using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Flow.Core.Abstractions;
using Flow.Core.Models.Context;
using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Results;
using Flow.Shared.Utils;

namespace Flow.Core.Runtime;

public class Executor : IDisposable
{
    private bool _completed;

    private INode? _current;

    private Function _function;

    [NotNull] private Queue<INode>? _currentPendingNodes;

    // To solve the problem of exiting directly before asynchronous nodes have been executed.
    private int _executingNodesCount;

    private readonly Stack<KeyValuePair<Function, Queue<INode>>> _backupStack;

    private readonly SemaphoreSlim _semaphoreSlim = new(1);

    // TODO: Context manager? How it works???
    private readonly InstanceManager<Guid> _instanceManager;

    public ProcessorStatus Status { get; private set; }

    public Result? Result { get; private set; }

    public Executor(Function function, InstanceManager<Guid>? instanceManager = null)
    {
        _currentPendingNodes = new Queue<INode>();
        _instanceManager = instanceManager ?? new InstanceManager<Guid>();
        _function = function ?? throw new ArgumentNullException(nameof(function));

        _backupStack = new Stack<KeyValuePair<Function, Queue<INode>>>();

        Status = ProcessorStatus.Ready;
    }

    #region Execution

    /// <summary>
    /// Check if this is the exit.
    /// </summary>
    /// <returns></returns>
    private bool IsExit(INode node)
        => Equals(_function.Exit, node);

    /// <summary>
    /// Execute the script.
    /// </summary>
    public async Task Execute()
    {
        if (_function.Entry is null)
            return;

        await Execute(_function.Entry);
    }

    /// <summary>
    /// Invoke a node and its subsequent nodes iteratively.
    /// </summary>
    /// <param name="node"></param>
    private async Task Execute(INode node)
    {
        _current = node;
        _currentPendingNodes.Enqueue(node);

        while (true)
        {
            await _semaphoreSlim.WaitAsync();

            if (_completed)
                break;

            if (Status.HasFlag(ProcessorStatus.Paused) || Status.HasFlag(ProcessorStatus.Cancelled))
                return;

            var n = _currentPendingNodes.Dequeue();

            if (IsExit(n))
            {
                ReturnToPreviousFunction();
                _semaphoreSlim.Release(1);
                continue;
            }

            await ExecuteSingle(n);
            _current = n;

            var nextIds = MoveNext(n);

            // If the node does not have a subsequent node.
            if (nextIds is null || nextIds.Length == 0) continue;

            // Enqueue subsequent nodes.
            EnqueueNodes(nextIds);
        }
    }

    /// <summary>
    /// Pass the arguments and invoke a single node.
    /// </summary>
    /// <param name="node">Single node to be invoked.</param>
    private ValueTask ExecuteSingle(INode node)
    {
        if (!node.IsEnabled)
            return ValueTask.CompletedTask;

        // If the node has unfilled values:
        if (NodeExtensions.GetUnfilledRequiredValues(node).Any())
        {
            if (!TryGetValueFromSource(node))
            {
                NodeExtensions.MarkAs(node, NodeStatus.Waiting);
                return ValueTask.CompletedTask;
            }
        }

        // If the node requires an instance.
        if (node is IInstanceRequired)
        {
            if (!TryPassContextToNode(node))
            {
                NodeExtensions.MarkAs(node, NodeStatus.Waiting);
                return ValueTask.CompletedTask;
            }
        }

        var success = true;
        node.Status |= NodeStatus.Running;

        // Attention:
        // If the same node implements both synchronous and asynchronous interfaces,
        // only the synchronous method will be executed.
        switch (node)
        {
            // If possible, execute this method on the node.
            case IExecutableNode en:
            {
                Interlocked.Increment(ref _executingNodesCount);
                try
                {
                    en.Execute();
                }
                catch (Exception ex)
                {
                    success = false;
                    OnNodeError(ex, en);
                }
                OnNodeCompleted(en);
                break;
            }

            // Execute asynchronously if the node implements the IAsyncExecutableNode.
            case IAsyncExecutableNode aen:
            {
                Interlocked.Increment(ref _executingNodesCount);
                aen.ExecuteAsync()
                    .Await(ex =>
                        {
                            success = false;
                            OnNodeError(ex, aen);
                        },
                        () =>
                        {
                            OnNodeCompleted(aen);
                        });
                break;
            }

            // Function node
            case IFunction fn:
            {
                SwitchToSubFunction(fn);
                break;
            }

            default:
                return ValueTask.CompletedTask;
        }

        if (success)
        {
            node.Status |= NodeStatus.Completed;
            node.Status |= NodeStatus.Ready;
        }

        if (!PassResults(node))
            Result = new Result(false, false, null, $"Value transfer error occurred on node: {node.RuntimeId}");

        return ValueTask.CompletedTask;
    }

    private void OnNodeError(Exception ex, INode node)
    {
        node.Status |= NodeStatus.Failed;
        var result = node.Result;
        Debug.WriteLine($"Exception detected: {ex.Message}, result: {result}");
    }

    private void OnNodeCompleted(INode node)
    {
        if (_completed) return;
        Interlocked.Decrement(ref _executingNodesCount);

        // When a (async) node completed its execution, we only need to check if the node is the exit.
        // If it is one exit:
        if (IsExit(node))
        {
            // Set the _completed to true.
            if (_executingNodesCount == 0 || _function.ExitImmediately)
                _completed = true;

            // And call function Execute(...) to finish.
            _semaphoreSlim.Release(1);
            return;
        }

        // It isn't:
        // Call function Execute(...) to execute.
        _semaphoreSlim.Release();
#if DEBUG
        Debug.WriteLine($"Completed node: {node.RuntimeId}\nSemaphoreSlim: {_semaphoreSlim.CurrentCount}");
#endif
    }

    private void EnqueueNodes(Guid[] nextIds)
    {
        foreach (var nextId in nextIds)
        {
            var next = _function.GetNode(nextId);
            _currentPendingNodes.Enqueue(next);
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

        _currentPendingNodes.Enqueue(node);
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
            var i = csn.ReturnedPort;
            return _function.GetRuntimeGuid(node) is not { } cid ? null : _function.GetNextProgressNodes(cid, i);
        }

        return _function.GetRuntimeGuid(node) is not { } id ? null : _function.GetNextProgressNodes(id);
    }

    /// <summary>
    /// Check the stack and return to origin function (caller).
    /// </summary>
    private void ReturnToPreviousFunction()
    {
        // _backupStack                             --return-> previous level
        // And if here's nothing to pop, which means the whole script was executed.
        if (!_backupStack.TryPop(out var pl))
        {
            _completed = true;
            return;
        }

        _function = pl.Key;

        // backup                                   --rejoin-> _currentPendingNodes
        _currentPendingNodes = Clone(pl.Value);

        // Pass results                             -> next
        PassResults(pl.Key);
    }

    /// <summary>
    /// Switch to function inside a function.
    /// </summary>
    /// <param name="node"><see cref="Function"/></param>
    private void SwitchToSubFunction(IFunction node)
    {
        if (node is not Function f)
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
        => new(nodes);

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

        var success = true;
        foreach (var p in targets)
        {
            var targetNode = _function.GetNode(p.NodeId);
            if (node is IFunction fn)
                targetNode = fn.Entry;

            var value = NodeExtensions.GetOutput(node, p.Index);
            if (!NodeExtensions.Assign(targetNode, p.Index, value))
            {
                success = false;
                continue;
            }

            TryEnqueueIfNodeIsWaiting(targetNode);
        }

        return success;
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

        var success = true;
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
                value = NodeExtensions.GetOutput(sn, p.Source.Index);
            }

            if (!NodeExtensions.Assign(node, p.Target.Index, value))
                success = false;
        }

        return success;
    }

    /// <summary>
    /// Try to get a context from the InstanceManager, and pass it to the node.
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
            ((IInstanceRequired)node).Instance = _instanceManager.TryFindContextObject(rc);
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
                                                 || source.InputVariableMetadata.Length !=
                                                 target.InputVariableMetadata.Length)
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
            Execute().Await();
            return;
        }

        Execute(_current).Await();
    }

    private void Stop()
    {
        Status |= ProcessorStatus.Cancelled;
        _completed = true;
        Dispose();
    }

    #endregion

    public void Dispose()
    {
        _instanceManager.Dispose();
        _function.Dispose();
        _semaphoreSlim.Dispose();

        GC.SuppressFinalize(this);
    }
}