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
public readonly struct VoidResult : IResult<bool, Exception>
{
    private readonly bool _isOk = false;

    private readonly bool _value = false;

    private readonly Exception? _err = null;

    /// <inheritdoc/>
    public bool IsOk => _isOk;

    /// <inheritdoc/>
    public bool IsErr => !IsOk;

    /// <inheritdoc/>
    public bool Value => _value;

    /// <inheritdoc/>
    public Exception? Error => _err;

    private VoidResult(bool isSuccess = false)
    {
        _isOk = true;
        _value = isSuccess;
    }

    private VoidResult(Exception? err)
    {
        _isOk = false;
        _value = false;
        _err = err;
    }

    public static VoidResult Ok(bool value = true)
        => new(value);

    public static VoidResult Err(Exception? err)
        => new(err);

    public static VoidResult Completed(bool isSuccess = false) 
        => new(isSuccess);

    /// <inheritdoc/>
    public bool Unwrap()
    {
        if (!_isOk)
            throw new InvalidOperationException("Called Unwrap on Err");
        return _value;
    }

    /// <inheritdoc/>
    public Exception UnwrapErr()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Result<TResult, Exception> Map<TResult>(Func<bool, TResult> map)
        where TResult : notnull
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Result<bool, F> MapErr<F>(Func<Exception, F> map)
        where F : notnull
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Match(Action<Result<bool, Exception>> ok, Action<Result<bool, Exception>> err)
    {
        throw new NotImplementedException();
    }

    public void Match(Action<VoidResult> ok, Action<VoidResult> err)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Converting <see cref="VoidResult"/> to common <see cref="Result"/> object.
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static implicit operator Result<bool, Exception>(VoidResult result)
        => result.IsOk
            ? Result<bool, Exception>.Ok(result.Value)
            : Result<bool, Exception>.Err(result.Error);

    /// <summary>
    /// Converting specific <see cref="Result"/> which T is bool and E is Exception
    /// to <see cref="VoidResult"/>. 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static implicit operator VoidResult(Result<bool, Exception> result)
        => result.IsOk
            ? Ok(result.Value)
            : Err(result.Error);

    /// <inheritdoc/>
    public IResult<bool, Exception> Clone()
        => _value switch
        {
            true => Ok(true),
            false => Ok(),
        };
}