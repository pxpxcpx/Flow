namespace Flow.Shared.Abstractions;

/// <summary>
/// Defines the interface for an asynchronously executable object.
/// </summary>
public interface IAsyncExecutable
{
    /// <summary>
    /// Execute method asynchronously.
    /// </summary>
    /// <returns></returns>
    Task ExecuteAsync();
}