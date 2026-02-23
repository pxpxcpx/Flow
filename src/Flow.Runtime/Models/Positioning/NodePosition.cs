namespace Flow.Runtime.Models.Positioning;

public record struct NodePosition(Guid NodeId, int? Index)
{
    public Guid NodeId { get; set; } = NodeId;

    public int? Index { get; set; } = Index ?? 0;
}
