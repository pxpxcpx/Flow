using Flow.Runtime.Models;

namespace Flow.Runtime.Abstractions;

/// <summary>
/// Record the connection between <see cref="ContextItem"/> items and <see cref="IInstanceRequired"/>.
/// </summary>
public record InstanceConnection
{
    public required Guid InstanceId { get; set; }
    
    public required NodePosition Node { get; set; }
}