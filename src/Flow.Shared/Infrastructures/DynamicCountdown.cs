namespace Flow.Shared.Infrastructures;

/// <summary>
/// Dynamic counting support for async tasks. Is similar to <see cref="CountdownEvent"/>.
/// Can't be reset, use <see cref="Add"/> to add counting.
/// </summary>
public sealed class DynamicCountdown
{
    private volatile int _currentCount;

    /// <summary>
    /// To prevent the deadlock and race condition.
    /// </summary>
    private readonly object _lock;

    /// <summary>
    /// Manually controlled task.
    /// <c>true</c> means countdown is over; <c>false</c> means countdown still has remaining counts.
    /// </summary>
    private TaskCompletionSource<bool> _tcs;

    /// <summary>
    /// Counts remaining.
    /// </summary>
    public int CurrentCount
    {
        get
        {
            lock (_lock)
                return _currentCount < 0 ? 0 : _currentCount;
        }
    }

    /// <summary>
    /// Initial count of the countdown.
    /// </summary>
    public int InitialCount { get; init; }

    /// <summary>
    /// Indicates whether the countdown has been set or not.
    /// </summary>
    public bool IsSet => _currentCount <= 0;

    public DynamicCountdown(int initialCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(initialCount);

        InitialCount = initialCount;

        _lock = new object();
        _tcs = CreateCompletedSource();
    }

    /// <summary>
    /// Increase by a certain amount.
    /// </summary>
    /// <param name="amount"></param>
    /// <exception cref="ArgumentOutOfRangeException"><see cref="amount"/> is negative or 0.</exception>
    public void Add(int amount = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

        lock (_lock)
        {
            if (_currentCount == 0)
                _tcs = CreatePendingSource();

            Interlocked.Add(ref _currentCount, amount);
        }
    }

    /// <summary>
    /// Decrease by a certain amount.
    /// </summary>
    /// <param name="amount"></param>
    /// <exception cref="ArgumentOutOfRangeException"><see cref="amount"/> is negative or 0.</exception>
    public void Signal(int amount = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);

        TaskCompletionSource<bool>? tcs = null;
        lock (_lock)
        {
            if (_currentCount < amount)
                throw new InvalidOperationException($"Signal {amount} called but only {_currentCount} available");

            Interlocked.Add(ref _currentCount, -amount);

            // check if there are no more counts left
            if (_currentCount == 0)
                tcs = _tcs;
        }

        // avoid deadlock
        tcs?.TrySetResult(true);
    }

    /// <summary>
    /// Asynchronous waiting count is reset to zero.
    /// </summary>
    public async Task<bool> WaitAsync(TimeSpan? timeout = null, CancellationToken? cancellationToken = null)
    {
        // task from the completion source will indicate whether the count is 0
        Task waitTask;
        lock (_lock)
        {
            waitTask = _tcs.Task;
        }

        if (waitTask.IsCompleted)
            return true;

        // wait async for timeout
        var delayTask = Task.Delay(timeout ?? TimeSpan.MaxValue, cancellationToken ?? CancellationToken.None);
        var completedTask = await Task.WhenAny(waitTask, delayTask).ConfigureAwait(false);

        if (completedTask == waitTask)
            return true;
        if (waitTask.IsCompleted)
            return true;

        cancellationToken?.ThrowIfCancellationRequested();
        return false;
    }

    /// <summary>
    /// Block and wait synchronously until the countdown reaches zero.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public bool Wait(TimeSpan? timeout = null, CancellationToken? cancellationToken = null)
        => WaitAsync(timeout, cancellationToken).GetAwaiter().GetResult();

    private static TaskCompletionSource<bool> CreatePendingSource()
        // Option "RunContinuationsAsynchronously" can prevent the synchronous execution
        // of the waiters' continuations when calling SetResult,
        // avoiding the blocking of the caller of Signal for a long time, and also preventing certain deadlock scenarios.
        => new(TaskCreationOptions.RunContinuationsAsynchronously);

    private static TaskCompletionSource<bool> CreateCompletedSource()
    {
        var tcs = new TaskCompletionSource<bool>();
        tcs.SetResult(true);
        return tcs;
    }
}