using Flow.Engine.Abstractions;
using Flow.Engine.ContextManager;
using Flow.PDK.Node.Internal;

namespace Flow.Engine;

public class Executor
{
    private IInternalNode _current;
    private IEnumerable<IInternalNode> _nodes;
    private ContextManager<Guid> _contextManager;

    public IScript Script { get; set; }
    
    public object? Result { get; private set; }

    public Executor(IScript script,
                    ContextManager<Guid>? contextManager = null,
                    bool positiveOrder = true)
    {
        _contextManager = contextManager ?? new ContextManager<Guid>();
        Script = script ?? throw new ArgumentNullException(nameof(script));
        _nodes = Script.Nodes ?? throw new ArgumentNullException(nameof(Script.Nodes));
    }

    #region Utilities

    protected IInternalNode GetCurrentNode()
    {
        throw new NotImplementedException();
    }
    
    protected IInternalNode GetProcessingNext()
    {
        throw new NotImplementedException();
    }
    
    protected IInternalNode GetProcessingPrevious()
    {
        throw new NotImplementedException();
    }

    #endregion

    public void Execute()
    {
        throw new NotImplementedException();
    }
}