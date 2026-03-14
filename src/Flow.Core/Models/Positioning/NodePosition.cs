namespace Flow.Core.Models.Positioning;

public record struct NodePosition(Guid NodeId)
{
    public Guid NodeId { get; set; } = NodeId;
}
