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
    private bool _completed;

    private INode? _current;

    private Function _function;

    [NotNull] private Queue<INode>? _currentPendingNodes;

    private readonly Stack<KeyValuePair<Function, Queue<INode>>> _backupStack;

    private readonly SemaphoreSlim _semaphoreSlim = new(1);

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
    }

    private Executor(IFunctionNode functionNode)
    {
        _currentPendingNodes = new Queue<INode>();
    }

    #region Execution

    /// <summary>
    /// Check if this is the exit.
    /// </summary>
    /// <returns></returns>
    public bool IsExit(INode node)
        => _function.Exit == node;

    /// <summary>
    /// Execute the script.
    /// </summary>
    public async Task Execute()
    {
        if (_function.Entry is null) return;
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

        while (!_completed)
        {
            await _semaphoreSlim.WaitAsync();

            if (Status.HasFlag(ProcessorStatus.Paused) || Status.HasFlag(ProcessorStatus.Cancelled))
                return;

            if (IsExit(node))
            {
                if (node is IFunctionNode)
                {
                    ReturnToPreviousFunction();
                    _semaphoreSlim.Release(1);
                    continue;
                }

                _completed = true;
                return;
            }

            var n = _currentPendingNodes.Dequeue();
            await ExecuteSingle(n);
            _semaphoreSlim.Release(1);
            _current = n;

            var nextIds = MoveNext(node);

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
        // If the node has unfilled values:
        if (node.GetUnfilledRequiredValues().Any())
        {
            if (!TryGetValueFromSource(node))
            {
                node.MarkAs(NodeStatus.Waiting);
                return ValueTask.CompletedTask;
            }
        }

        // If the node requires an instance.
        if (node is IInstanceRequired)
        {
            if (!TryPassContextToNode(node))
            {
                node.MarkAs(NodeStatus.Waiting);
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
                try
                {
                    en.Execute();
                }
                catch (Exception ex)
                {
                    success = false;
                    OnNodeError(ex, en);
                }

                break;
            }

            // Execute asynchronously if the node implements the IAsyncExecutableNode.
            case IAsyncExecutableNode aen:
            {
                aen.ExecuteAsync()
                    .Await(ex =>
                        {
                            success = false;
                            OnNodeError(ex, aen);
                        },
                        () => { OnNodeCompleted(aen); });
                break;
            }

            // Function node
            case IFunctionNode fn:
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

        // When a (async) node completed its execution, we only need to check if the node is the exit.
        // If it is one exit:
        if (IsExit(node))
        {
            // Set the _completed to true.
            _completed = true;

            // And call function Execute(...) to finish.
            _semaphoreSlim.Release(1);
            return;
        }

        // It isn't:
        // Enqueue next nodes.
        EnqueueSubsequentNode(node);
        // Call function Execute(...) to execute.
        _semaphoreSlim.Release(1);
    }

    private void EnqueueSubsequentNode(INode node)
    {
        var nextIds = MoveNext(node);
        EnqueueNodes(nextIds!);
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
        {
            _completed = true;
            return;
        }
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

        var success = true;
        foreach (var p in targets)
        {
            var targetNode = _function.GetNode(p.NodeId);
            if (node is IFunctionNode fn)
                targetNode = fn.Entry;

            var value = node.GetOutput(p.Index);
            if (!targetNode.Assign(p.Index, value))
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
                value = sn.GetOutput(p.Source.Index);
            }

            if (node.Assign(p.Target.Index, value))
                success = false;
        }

        return success;
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