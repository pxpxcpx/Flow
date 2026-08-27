namespace Flow.Shared.Abstractions;

/// <summary>
/// Generic type container for multiple params.
/// </summary>
public interface IParamCollection
{
    int Count { get; }
    
    T Get<T>(int index);
    
    void Set<T>(int index, T value);
}