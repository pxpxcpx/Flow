using System.Diagnostics;

namespace Flow.Shared.Infrastructures;

/// <summary>
/// A controller similar to <see cref="CancellationTokenSource"/>
/// for implementing process pause and resume functionality.
/// </summary>
/// <seealso cref="PauseTokenSource"/>
[DebuggerDisplay("PauseTokenSource.IsPaused: {IsPaused}")]
public sealed class PauseTokenSource
{
    private readonly object _lock = new();

    /// <summary>
    /// <c>null</c> when the pause hasn't been triggered or in default state.
    /// Is used to control the internal task wait  for the restoration.
    /// When <seealso cref="TaskCompletionSource{T}.TrySetResult"/> called, the waiting will be over,
    /// and _tcs will be reset into null again.
    /// </summary>
    private TaskCompletionSource<bool>? _tcs;

    /// <summary>
    /// Indicates whether the pause has benn triggered.
    /// </summary>
    public bool IsPaused
    {
        get
        {
            lock (_lock)
                return _tcs is not null;
        }
    }

    /// <summary>
    /// Get a new <see cref="PauseToken"/> controlled by this instance.
    /// </summary>
    public PauseToken Token
        => new(this);

    /// <summary>
    /// Pause the process.
    /// </summary>
    public void Pause()
    {
        lock (_lock)
        {
            // already paused
            if (_tcs is not null)
                return;

            // not pause yet
            _tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        }
    }

    /// <summary>
    /// Restore the process.
    /// </summary>
    public void Resume()
    {
        TaskCompletionSource<bool>? tcs;

        lock (_lock)
        {
            tcs = _tcs;
            _tcs = null;
        }

        tcs?.TrySetResult(true);
    }

    /// <summary>
    /// After the pause is triggered, wait asynchronously until the restoration occurs.
    /// </summary>
    /// <param name="ct">Token to cancel the pause.</param>
    internal ValueTask WaitWhilePausedAsync(CancellationToken ct)
    {
        Task? waitTask;

        lock (_lock)
            waitTask = _tcs?.Task;

        // The pause has ended.
        if (waitTask is null || waitTask.IsCompletedSuccessfully)
            return ValueTask.CompletedTask;

        // Otherwise, wait.
        return new ValueTask(waitTask.WaitAsync(ct));
    }
}