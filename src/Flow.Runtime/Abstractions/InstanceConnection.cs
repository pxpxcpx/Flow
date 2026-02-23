using Flow.Runtime.Models.Positioning;

namespace Flow.Runtime.Abstractions;

/// <summary>
/// Connection between context items and IInstanceRequired nodes.
/// </summary>
public record struct InstanceConnection
{
    public required Guid InstanceId { get; set; }
    
    public required NodePosition Node { get; set; }
}