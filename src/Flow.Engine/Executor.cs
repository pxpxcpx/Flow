using Flow.Engine.Abstractions;
using Flow.Engine.ContextManager;
using Flow.SDK.Plugins.Node;
using Flow.SDK.Plugins.Node.Internal;

namespace Flow.Engine;

public class Executor
{
    private INode _current;
    private IEnumerable<INode> _nodes;
    private ContextManager<Guid> _contextManager;

    public IScript Script { get; set; }
    
    public object? Result { get; private set; }

    public Executor(IScript script,
                    ContextManager<Guid>? contextManager = null)
    {
        _contextManager = contextManager ?? new ContextManager<Guid>();
        Script = script ?? throw new ArgumentNullException(nameof(script));
        _nodes = Script.Nodes ?? throw new ArgumentNullException(nameof(Script.Nodes));
    }

    #region Utilities

    protected INode GetCurrentNode()
    {
        throw new NotImplementedException();
    }
    
    protected INode GetProcessingNext()
    {
        throw new NotImplementedException();
    }
    
    protected INode GetProcessingPrevious()
    {
        throw new NotImplementedException();
    }

    #endregion

    public void Execute()
    {
        throw new NotImplementedException();
    }
}