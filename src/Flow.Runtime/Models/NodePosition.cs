namespace Flow.Runtime.Models;

public record struct NodePosition
{
    public Guid Position { get; set; }
    
    public int Index { get; set; }
}
