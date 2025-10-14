using Flow.Engine.Abstractions;
using Flow.Engine.ContextManager;
using Flow.Engine.Utils;
using Flow.SDK.Plugins.Node;

namespace Flow.Engine;

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

    public void Run()
        => InvokeSingle(Script.Entry);

    private void InvokeSingle(INode node)
    {
        var entryRuntimeId = node.GetRuntimeGuid(Script);
        if (entryRuntimeId is not { } rtId) return;
        var nextIds = MoveNext(rtId);
        if (nextIds is null || nextIds.Length == 0) return;
        
        node.Execute();
        
        foreach (var n in nextIds)
        {
            var next = n.GetNode(Script);
            InvokeSingle(next);
        }
    }

    private void PassParameters(INode node)
    {
        throw new NotImplementedException();
    }
    
    private bool CheckRequiredValues(INode node)
    {
        throw new NotImplementedException();
    }
    
    private Guid[]? MoveNext(Guid currentGuid) 
        => currentGuid.GetNextProgressNodes(Script);
}