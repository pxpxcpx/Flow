using Flow.Runtime.Models;

namespace Flow.Runtime.Abstractions;

/// <summary>
/// Record the connection between context items and IInstanceRequired nodes.
/// </summary>
public record InstanceConnection
{
    public required Guid InstanceId { get; set; }
    
    public required NodePosition Node { get; set; }
}