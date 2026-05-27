namespace Flow.Shared.Abstractions;

public interface IPluggable : IDisposable
{
    /// <summary>
    /// Initialize dependency, services, resources, etc.
    /// </summary>
    /// <returns></returns>
    Task Initialize();
    
    object?[]? Dependencies { get; }
}