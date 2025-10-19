using Flow.Runtime.Utils;
using Flow.Runtime.Abstractions;
using Flow.Runtime.ContextManager;
using Flow.SDK.Plugins.Node;

namespace Flow.Runtime;

public class Executor
{
    private INode _current;

    private ContextManager<Guid> _contextManager;

    public IScript Script { get; set; }

    public object? Result { get; private set; }

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

        InvokeRecursively(Script.Entry);
    }

    /// <summary>
    /// Invoke a node and its subsequent nodes recursively.
    /// </summary>
    /// <param name="node"></param>
    private void InvokeRecursively(INode node)
    {
        _current = node;

        InvokeSingle(node);
        
        // If the node does not have a subsequent node (or the executor reaches to the end), return.
        var nextIds = MoveNext(node);
        if (nextIds is null || nextIds.Length == 0) return;
        
        // Get subsequent nodes and recursively invoke them.
        foreach (var n in nextIds)
        {
            var next = n.GetNode(Script);
            InvokeRecursively(next);
        }
    }

    /// <summary>
    /// Pass the arguments and invoke a single node.
    /// </summary>
    /// <param name="node">Single node to be invoked.</param>
    private void InvokeSingle(INode node)
    {
        // Pass arguments to the node.
        if (node.GetUnfilledRequiredValues() is not { } unfilled)
            TryGetValueFromConnection(node);

        node.Execute();

        PassResults(node);
    }

    /// <summary>
    /// Get results from a node, and pass them according to connections. (Actively pass values)
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private bool PassResults(INode node)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get a value from a related connection. (Passive value retrieval)
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private bool TryGetValueFromConnection(INode node)
    {
        throw new NotImplementedException();
    }

    private Guid[]? MoveNext(INode node)
        => node.GetRuntimeGuid(Script) is not { } id ? null : id.GetNextProgressNodes(Script);
}