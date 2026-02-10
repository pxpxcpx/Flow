using Flow.Runtime.Abstractions;

namespace Flow.Runtime.Models;

/// <summary>
/// Process connection between two nodes.
/// </summary>
public record struct ProcessConnection(NodePosition Source, NodePosition Target) : IProcessConnection
{
    public required NodePosition Source { get; set; } = Source;

    public required NodePosition Target { get; set; } = Target;
}