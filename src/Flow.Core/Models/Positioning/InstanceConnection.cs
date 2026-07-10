namespace Flow.Core.Models.Positioning;

/// <summary>
/// Connection between context items and IInstanceRequired nodes.
/// </summary>
public record struct InstanceConnection
{
    public required Guid InstanceId { get; set; }
    
    public required NodePort Node { get; set; }
}