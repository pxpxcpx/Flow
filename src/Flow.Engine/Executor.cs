using Flow.Engine.Abstractions;
using Flow.PDK.Node.Internal;

namespace Flow.Engine;

public class Executor
{
    private IInternalNode _current;
    private List<IInternalNode> _nodes = new();

    public IScript Script { get; set; }

    public Executor(IScript script)
    {
        Script = script ?? throw new ArgumentNullException(nameof(script));
        _nodes = (List<IInternalNode>)Script.Nodes ?? throw new ArgumentNullException(nameof(Script.Nodes));
    }

    private IInternalNode GetNext()
    {
        throw new NotImplementedException();
    }

    public void Execute()
    {
        throw new NotImplementedException();
    }
}