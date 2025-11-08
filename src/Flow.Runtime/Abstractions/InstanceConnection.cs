using Flow.Runtime.ContextManager;
using Flow.Runtime.Models;
using Flow.Shared.Abstractions;

namespace Flow.Runtime.Abstractions;

/// <summary>
/// Record the connections between <see cref="ContextItem"/> items and <see cref="IInstanceRequired"/>.
/// </summary>
public record InstanceConnection
{
    public required Guid InstanceId { get; set; }
    
    public required NodePosition Node { get; set; }
}