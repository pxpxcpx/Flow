namespace Flow.Runtime.Models;

public record struct VariablePosition
{
    public int Index { get; set; }

    public Guid Node { get; set; }
}
