namespace Flow.Shared.Utils;

/// <summary>
/// Extension/static methods for <see cref="Task"/>.
/// </summary>
public static class TaskHelper
{
    /// <summary>
    /// Fire and forget safely.
    /// Used to call asynchronous <see cref="Task"/> methods within synchronous methods.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="onError">Error handler.</param>
    /// <param name="onCompleted">Will be invoked when task complete.</param>
    public static async void Await(
        this Task task, Action<Exception>? onError = null, Action? onCompleted = null)
    {
        try
        {
            await task;
            onCompleted?.Invoke();
        }
        catch (Exception ex)
        {
            onError?.Invoke(ex);
        }
    }
}