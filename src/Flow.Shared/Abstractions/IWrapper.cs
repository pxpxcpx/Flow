namespace Flow.Shared.Abstractions;

public interface IWrapper
{
    Type Type { get; }
    
    object Object { get; }
}