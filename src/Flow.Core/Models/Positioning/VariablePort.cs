namespace Flow.Core.Models.Positioning;

public record struct VariablePort
{
    public int Index { get; set; }

    public Guid NodeId { get; set; }
}
