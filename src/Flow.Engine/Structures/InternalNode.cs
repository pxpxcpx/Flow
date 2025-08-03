using Flow.PDK.Node;
using Flow.PDK.Node.Internal;
using Flow.PDK.Node.Internal.Enums;

namespace Flow.Engine.Structures;

internal class InternalNode : IInternalNode
{
    public Guid Id { get; init; }
    public Guid RuntimeId { get; init; }
    public string Description { get; set; }
    public string Name { get; set; }
    
    public NodeStatus Status { get; set; }
    
    public Dictionary<int, string> Inputs { get; set; }
    public Dictionary<int, string> Outputs { get; set; }
    
    public IExecutable Executable { get; set; }

    public InternalNode() : this(string.Empty)
    {
    }

    public InternalNode(string description)
    {
        Id = Guid.NewGuid();
        Description = description;
    }

    public void Execute()
    {
        throw new NotImplementedException();
    }
}
