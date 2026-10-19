namespace Flow.Shared.Infrastructures;

/// <summary>
/// A token similar to <seealso cref="CancellationToken"/>
/// for control process pause and resume.
/// </summary>
/// <seealso cref="PauseTokenSource"/>
public readonly struct PauseToken
{
    private readonly PauseTokenSource? _source;

    /// <summary>
    /// Indicates whether the pause has benn triggered.
    /// </summary>
    public bool IsPaused => _source is { IsPaused: true };

    internal PauseToken(PauseTokenSource source)
    {
        _source = source;
    }

    /// <summary>
    /// After the pause is triggered, wait asynchronously until the restoration occurs.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the pause.</param>
    /// <returns></returns>
    public ValueTask WaitWhilePausedAsync(CancellationToken cancellationToken) 
        => _source?.WaitWhilePausedAsync(cancellationToken) ?? ValueTask.CompletedTask;
}