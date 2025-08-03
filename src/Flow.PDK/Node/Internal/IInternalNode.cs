using Flow.PDK.Node.Internal.Enums;

namespace Flow.PDK.Node.Internal;

public interface IInternalNode : INode
{
    /// <summary>
    /// ID when the node was created in the script, differs from <see cref="INode.Id"/>
    /// </summary>
    Guid RuntimeId { get; init; }

    NodeStatus Status { get; set; }
    
    Dictionary<int, string> Inputs { get; set; }
    
    Dictionary<int, string> Outputs { get; set; }

    IExecutable Executable { get; set; }
}
