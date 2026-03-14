namespace Flow.Core.Models.Positioning;

public record struct VariablePosition
{
    public int Index { get; set; }

    public Guid NodeId { get; set; }
}
