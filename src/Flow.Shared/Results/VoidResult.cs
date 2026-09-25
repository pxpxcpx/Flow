using Flow.Shared.Abstractions;

namespace Flow.Shared.Results;

/// <summary>
/// Indicates an operation's result.
/// <example>
/// Contains 3 possible types:
/// - Completed and success:
/// <code>var success = VoidResult.Completed(true);</code>
/// - Completed but failed;
/// <code>var failed = VoidResult.Completed(false);</code>
/// - Error during the operation;
/// <code>var err = VoidResult.Error(...);</code>
/// </example>
/// </summary>
public record VoidResult : IResult<bool, Exception>
{
    private readonly bool _isCompleted = false;

    private readonly bool _isOk = false;

    private readonly Exception? _err = null;

    /// <inheritdoc/>
    [Obsolete("Use property IsCompleted instead.", true)]
    public bool IsOk => _isCompleted;
    
    /// <summary>
    /// Indicates whether the operation completed.
    /// </summary>
    /// <remarks>
    /// To determine whether the operation was succeeded,
    /// use <see cref="Value"/>
    /// </remarks>
    public bool IsCompleted => _isCompleted;

    /// <inheritdoc/>
    public bool IsErr => !_isCompleted;

    /// <summary>
    /// Indicates whether the operation succeeded.
    /// </summary>
    /// <remarks>
    /// To determine whether the operation was completed,
    /// use <see cref="IsCompleted"/>
    /// </remarks>
    public bool Value => _isOk;

    /// <inheritdoc/>
    public Exception? Error => _err;

    private VoidResult(bool isSuccess = false)
    {
        _isCompleted = true;
        _isOk = isSuccess;
    }

    private VoidResult(Exception? err)
    {
        _isCompleted = false;
        _isOk = false;
        _err = err;
    }

    public static VoidResult Ok()
        => new(true);

    public static VoidResult Err(Exception? err)
        => new(err);

    public static VoidResult Completed(bool isSuccess = false)
        => new(isSuccess);

    /// <inheritdoc/>
    public bool Unwrap()
    {
        if (!_isCompleted)
            throw new InvalidOperationException("Called Unwrap on Err");
        return _isOk;
    }

    /// <inheritdoc/>
    public Exception? UnwrapErr()
    {
        if(_isCompleted)
            throw new InvalidOperationException("Called UnwrapErr on Ok");
        return _err;
    }

    /// <inheritdoc/>
    public Result<TResult, Exception> Map<TResult>(Func<bool, TResult> map)
        where TResult : notnull
        => IsCompleted
            ? Result<TResult, Exception>.Ok(map(_isOk))
            : Result<TResult, Exception>.Err(_err);

    /// <inheritdoc/>
    public Result<bool, F> MapErr<F>(Func<Exception, F> map)
        where F : notnull
        => IsErr
            ? Result<bool, F>.Err(map(_err!))
            : Result<bool, F>.Ok(_isOk);

    /// <inheritdoc/>
    public void Match(Action<Result<bool, Exception>> ok, Action<Result<bool, Exception>> err)
    {
        if(IsCompleted)
            ok(this);
        else
            err(this);
    }

    /// <inheritdoc cref="IResult{T,E}.Match"/>
    public void Match(Action<VoidResult> ok, Action<VoidResult> err)
    {
        if(IsCompleted)
            ok(this);
        else
            err(this);
    }

    /// <summary>
    /// Converting <see cref="VoidResult"/> to common <see cref="IResult{T, T}"/> object.
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static implicit operator Result<bool, Exception>(VoidResult result)
        => result.IsCompleted
            ? Result<bool, Exception>.Ok(result.Value)
            : Result<bool, Exception>.Err(result.Error);

    /// <summary>
    /// Converting specific <see cref="IResult{T, T}"/> which T is bool and E is Exception
    /// to <see cref="VoidResult"/>. 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static implicit operator VoidResult(Result<bool, Exception> result) 
        => result.IsErr                // common Result<T, E> only have two values
            ? Err(result.Error)        // error
            : Completed(result.Value); // no error, means result is completed, and the value is the success status.

    /// <inheritdoc/>
    IResult<bool, Exception> ICloneable<IResult<bool, Exception>>.Clone()
        => _isOk switch
        {
            true => Ok(),
            false => Completed(),
        };
}