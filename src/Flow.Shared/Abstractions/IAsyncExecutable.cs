namespace Flow.Shared.Abstractions;

/// <summary>
/// Defines the interface for an asynchronously executable object.
/// </summary>
public interface IAsyncExecutable
{
    /// <summary>
    /// Execute method asynchronously.
    /// </summary>
    /// <remarks>
    /// To implement this method, it may be necessary
    /// to <b>manually implement parameter capture</b> to avoid context inconsistency.
    /// <example>
    /// Manually implement:
    /// <code>
    /// public Task ExecuteAsync()
    /// {
    ///     var snapshot = Inputs;
    ///     (Method body...)
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    /// <returns></returns>
    Task ExecuteAsync();
}