using Flow.Shared.Abstractions;

namespace Flow.Shared.Results;

/// <summary>
/// Represent a result, wrapped the result or error information.
/// </summary>
/// <typeparam name="T">Type of <see cref="Value"/></typeparam>
/// <typeparam name="E">Type of <see cref="Error"/></typeparam>
public readonly struct Result<T, E> : IResult<T, E>
    where T : notnull
    where E : notnull
{
    private readonly bool _isOk = false;

    private readonly T? _value = default;

    private readonly E? _err = default;

    /// <inheritdoc/>
    public bool IsOk => _isOk;

    /// <inheritdoc/>
    public bool IsErr => !_isOk;

    /// <inheritdoc/>
    public T? Value => _value;

    /// <inheritdoc/>
    public E? Error => _err;

    private Result(T? value)
    {
        _value = value;
        _err = default;
        _isOk = true;
    }

    private Result(E? err)
    {
        _value = default;
        _err = err;
        _isOk = false;
    }

    public static Result<T, E> Ok(T? value)
        => new(value);

    public static Result<T, E> Err(E? err)
        => new(err);

    /// <inheritdoc/>
    public T? Unwrap()
    {
        if (!_isOk)
            throw new InvalidOperationException("Called Unwrap on Err");
        return _value;
    }

    /// <inheritdoc/>
    public E? UnwrapErr()
    {
        if (_isOk)
            throw new InvalidOperationException("Called UnwrapErr on Ok");
        return _err;
    }

    /// <inheritdoc/>
    public IResult<T, E> Clone()
    {
        T newValue;

        switch (_value)
        {
            case ICloneable c:
                newValue = (T)c.Clone();
                break;
            case ICloneable<T> ct:
                newValue = ct.Clone();
                break;
            default:
                return (Result<T, E>)MemberwiseClone();
        }

        return Ok(newValue);
    }

    /// <inheritdoc/>
    public Result<TResult, E> Map<TResult>(Func<T, TResult> map)
        where TResult : notnull
        => IsOk
            ? Result<TResult, E>.Ok(map(_value!))
            : Result<TResult, E>.Err(_err!);

    /// <inheritdoc/>
    public Result<T, F> MapErr<F>(Func<E, F> map)
        where F : notnull
        => IsErr
            ? Result<T, F>.Err(map(_err!))
            : Result<T, F>.Ok(_value);

    /// <inheritdoc/>
    public void Match(Action<Result<T, E>> ok, Action<Result<T, E>> err)
    {
        if (IsOk)
            ok(this);
        else
            err(this);
    }
}

/// <summary>
/// An error-handling method used as an alternative to throwing exceptions,
/// which records the results and status of execution.
/// </summary>
/// <param name="IsCompleted"></param>
/// <param name="IsSuccess"></param>
/// <param name="Exception"></param>
/// <param name="Message"></param>
public readonly struct Result(
    bool IsCompleted,
    bool IsSuccess,
    Exception? Exception = null,
    string? Message = null)
{
    public readonly bool IsCompleted = IsCompleted;

    public readonly bool IsSuccess = IsSuccess;

    public readonly Exception? Exception = Exception;

    public readonly string? Message = Message ?? Exception?.Message;

    /// <summary>
    /// Result used to indicate “completion”.
    /// </summary>
    /// <remarks>Note that this does not indicate the task has been successfully executed.</remarks>
    public static Result Completed() => new(true, false);

    /// <summary>
    /// Result indicating “successfully completed”
    /// </summary>
    public static Result Success() => new(true, true);

    /// <summary>
    /// Result indicating “an error has occurred” with an empty exception and an empty message.
    /// </summary>
    public static Result Error() => new(false, false);
}