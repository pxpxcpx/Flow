namespace Flow.Shared.Abstractions;

public interface IInstanceRequired
{
    Type InstanceType { get; }
    
    object? Instance { get; set; }
}