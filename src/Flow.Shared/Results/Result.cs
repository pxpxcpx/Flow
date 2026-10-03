using Flow.Shared.Abstractions;

namespace Flow.Shared.Results;

/// <summary>
/// Represent a result, wrapped the result or error information.
/// </summary>
/// <typeparam name="T">Type of <see cref="Value"/></typeparam>
/// <typeparam name="E">Type of <see cref="Error"/></typeparam>
public record Result<T, E> : IResult<T, E>
    where T : notnull
    where E : notnull
{
    /// <inheritdoc/>
    public bool IsOk { get; }

    /// <inheritdoc/>
    public bool IsErr => !IsOk;

    /// <inheritdoc/>
    public T? Value { get; }

    /// <inheritdoc/>
    public E? Error { get; }

    protected Result()
    {
    }
    
    private Result(T? value)
    {
        Value = value;
        Error = default;
        IsOk = true;
    }

    private Result(E? err)
    {
        Value = default;
        Error = err;
        IsOk = false;
    }

    // Use those classes with pattern matching.
    
    public record Success : Result<T, E>
    {
        internal Success(T? value) : base(value)
        {
        }
    }

    public record Failure : Result<T, E>
    {
        internal Failure(E? err) : base(err)
        {
        }
    }

    public static Result<T, E> Ok(T? value)
        => new Success(value);

    public static Result<T, E> Err(E? err)
        => new Failure(err);

    /// <inheritdoc/>
    public T? Unwrap()
        => !IsOk
            ? throw new InvalidOperationException("Called Unwrap on Err")
            : Value;

    /// <inheritdoc/>
    public E? UnwrapErr()
        => IsOk
            ? throw new InvalidOperationException("Called UnwrapErr on Ok")
            : Error;

    // Members named "Clone" are disallowed in records.
    /// <inheritdoc/>
    IResult<T, E> ICloneable<IResult<T, E>>.Clone()
    {
        if (IsErr)
            return Err(Error);
        
        T newValue;

        switch (Value)
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
    public IResult<TResult, E> Map<TResult>(Func<T, TResult> map)
        where TResult : notnull
        => IsOk
            ? Result<TResult, E>.Ok(map(Value!))
            : Result<TResult, E>.Err(Error!);

    /// <inheritdoc/>
    public IResult<T, F> MapErr<F>(Func<E, F> map)
        where F : notnull
        => IsErr
            ? Result<T, F>.Err(map(Error!))
            : Result<T, F>.Ok(Value);

    /// <inheritdoc/>
    public void Match(Action<IResult<T, E>> ok, Action<IResult<T, E>> err)
    {
        if (IsOk)
            ok(this);
        else
            err(this);
    }
}