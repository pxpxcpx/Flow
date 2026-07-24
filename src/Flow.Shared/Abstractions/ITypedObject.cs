namespace Flow.Shared.Abstractions;

public interface ITypedObject
{
    Type Type { get; }
    
    object Object { get; }
}