namespace Flow.Core.Models.Positioning;

public record struct NodePort(Guid NodeId)
{
    public Guid NodeId { get; set; } = NodeId;
}
