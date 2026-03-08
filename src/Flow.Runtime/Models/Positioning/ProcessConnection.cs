using Flow.Runtime.Abstractions;

namespace Flow.Runtime.Models.Positioning;

/// <summary>
/// Process connection between two nodes.
/// </summary>
public record struct ProcessConnection(NodePosition Source, NodePosition Target, int ProcessIndex = 0) : IProcessConnection
{
    public required NodePosition Source { get; set; } = Source;

    public int ProcessIndex { get; set; } = ProcessIndex;

    public required NodePosition Target { get; set; } = Target;
}