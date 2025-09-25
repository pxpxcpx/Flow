namespace Flow.Engine.Models;

public record struct VariablePosition
{
    public int Position { get; set; }

    public Guid Node { get; set; }
}
