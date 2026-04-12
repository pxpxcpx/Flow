using Flow.Core.Models.Positioning;

namespace Flow.Core.Abstractions;

/// <summary>
/// Connection between context items and IInstanceRequired nodes.
/// </summary>
public record struct InstanceConnection
{
    public required Guid InstanceId { get; set; }
    
    public required NodePort Node { get; set; }
}