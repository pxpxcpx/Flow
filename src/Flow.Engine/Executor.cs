using Flow.Engine.Abstractions;

namespace Flow.Engine;

public class Executor
{
    private INode _current;
    private List<INode> _nodes = new List<INode>();

    public IScript Script { get; set; }

    public Executor(IScript script)
    {
        Script = script ?? throw new ArgumentNullException(nameof(script));
        _nodes = (List<INode>)Script.Nodes ?? throw new ArgumentNullException(nameof(Script.Nodes));
    }

    private INode GetNext()
    {
        throw new NotImplementedException();
    }

    public void Execute()
    {
        Script.Initialize();
        Script.StartNode.Execute();
    }
}