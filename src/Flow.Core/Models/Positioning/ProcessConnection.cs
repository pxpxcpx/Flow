using Flow.Core.Abstractions.Interfaces;

namespace Flow.Core.Models.Positioning;

/// <summary>
/// Process connection between two nodes.
/// </summary>
public record struct ProcessConnection(NodePort Source, NodePort Target, int Port = 0) : IProcessConnection
{
    public required NodePort Source { get; set; } = Source;

    public int Port { get; set; } = Port;

    public required NodePort Target { get; set; } = Target;
}