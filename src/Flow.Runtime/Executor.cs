using System.Diagnostics;
using Flow.Runtime.Utils;
using Flow.Runtime.Abstractions;
using Flow.Runtime.ContextManager;
using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Results;

namespace Flow.Runtime;

public class Executor
{
    private INode _current;

    private ContextManager<Guid> _contextManager;

    public IScript Script { get; set; }

    public Result? Result { get; private set; }

    public Executor(IScript script, ContextManager<Guid>? contextManager = null)
    {
        _contextManager = contextManager ?? new ContextManager<Guid>();
        Script = script ?? throw new ArgumentNullException(nameof(script));
    }

    /// <summary>
    /// Execute the script.
    /// </summary>
    public void Execute()
    {
        // If the entry is null (or simply, the script is empty), return.
        if (Script.Entry is null)
            return;

        ExecuteIteratively(Script.Entry);
    }

    /// <summary>
    /// Invoke a node and its subsequent nodes recursively.
    /// </summary>
    /// <param name="node"></param>
    private void ExecuteIteratively(INode node)
    {
        // Process the entry.
        _current = node;
        var q = new Queue<INode>();
        q.Enqueue(node);
        ExecuteSingle(node);
        
        while (q.Count > 0)
        {
            var n = q.Dequeue();
            _current = n;
            ExecuteSingle(n);
            
            // If the node does not have a subsequent node (or the executor reaches to the end), return.
            var nextIds = MoveNext(node);
            if (nextIds is null || nextIds.Length == 0) return;
        
            // Get subsequent nodes and recursively invoke them.
            foreach (var nextId in nextIds)
            {
                var next = nextId.GetNode(Script);
                q.Enqueue(next);
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
            if (!TryPassValueFromSource(node))
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
        
        if (succeed) return;
        PassResults(node);
        return;

        void OnError(Exception ex)
        {
            succeed = false;
            var result = node.Result;
            Debug.WriteLine($"Exception detected: {ex.Message}, result: {result}");
        }
    }

    /// <summary>
    /// Used to actively attempt to invoke nodes
    /// marked as <see cref="NodeStatus.Waiting"/> after parameters have been passed.
    /// </summary>
    /// <param name="node"></param>
    private void CallIfNodeIsWaiting(INode node)
    {
        if (node.Status != NodeStatus.Waiting)
            return;
        
        ExecuteSingle(node);
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
            return node.GetRuntimeGuid(Script) is not { } cid ? null : cid.GetNextProgressNodes(Script, i);
        }
        
        return node.GetRuntimeGuid(Script) is not { } id ? null : id.GetNextProgressNodes(Script);
    }

    #region Variable Utils
    
    /// <summary>
    /// Get results from a node, and pass them according to connections. (Actively pass values)
    /// </summary>
    /// <param name="node">The node has completed processing and is ready to transmit the results to the next node</param>
    /// <returns></returns>
    private bool PassResults(INode node)
    {
        if (node.GetRuntimeGuid(Script) is not { } rt) return false;
        if (rt.GetVariableTarget(Script) is not { } targets) return false;

        var succeed = true;
        foreach (var p in targets)
        {
            var targetNode = p.Node.GetNode(Script);
            var value = node.GetOutput(p.Index);
            if (!targetNode.Assign(p.Index, value))
            {
                succeed = false;
                continue;
            }
            CallIfNodeIsWaiting(targetNode);
        }

        return succeed;
    }

    /// <summary>
    /// Get a result from a related value source. (Passive value retrieval)
    /// </summary>
    /// <param name="node">Value source node</param>
    /// <returns></returns>
    private bool TryPassValueFromSource(INode node)
    {
        if (node.GetRuntimeGuid(Script) is not { } rt) return false;
        if (rt.GetVariableSource(Script) is not { } sources) return false;

        var succeed = true;
        foreach (var p in sources)
        {
            var targetNode = p.Node.GetNode(Script);
            var value = node.GetOutput(p.Index);
            if (!targetNode.Assign(p.Index, value))
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
        if (node.GetRuntimeGuid(Script) is not { } rt) return false;
        if (rt.GetRelatedContext(Script) is not { } rc) return false;

        try
        {
            ((IInstanceRequired)node).Instance = Script.ContextManager.TryFindContextObject(rc);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    
    #endregion

    #region Process Control

    private void Pause()
    {
        throw new NotImplementedException();
    }

    private void Resume()
    {
        throw new NotImplementedException();
    }

    private void Stop()
    {
        throw new NotImplementedException();
    }

    #endregion
}
