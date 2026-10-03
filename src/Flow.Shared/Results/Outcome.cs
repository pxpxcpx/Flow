using Flow.Shared.Abstractions;

namespace Flow.Shared.Results;

// I don't really sure if this could instead the common return value?
// Let it be an abstract class or just a template may be better?

/// <summary>
/// Result with only one type of both value and error.
/// </summary>
/// <typeparam name="T">Type of the value and the error.</typeparam>
public abstract record Outcome<T> : IResult<T, T>
    where T : notnull
{
    /// <inheritdoc/>
    public bool IsOk { get; }

    /// <inheritdoc/>
    public bool IsErr => !IsOk;

    /// <inheritdoc/>
    public T? Value { get; }

    /// <inheritdoc/>
    public T? Error { get; }

    private Outcome(bool isOk, T? valueOrErr)
    {
        IsOk = isOk;
    }
    
    // Use those classes with pattern matching.
    
    public record Success : Outcome<T>
    {
        internal Success(T? value) : base(true, value)
        {
        }
    }

    public record Failure : Outcome<T>
    {
        internal Failure(T? err) : base(false, err)
        {
        }
    }
    
    public static Success Ok(T? value)
        => new(value);
    
    public static Failure Err(T? err)
        => new(err);

    /// <inheritdoc/>
    public T? Unwrap()
        => !IsOk
            ? throw new InvalidOperationException("Called Unwrap on Err")
            : Value;

    /// <inheritdoc/>
    public T? UnwrapErr()
        => IsOk
            ? throw new InvalidOperationException("Called UnwrapErr on Ok")
            : Error;

    /// <inheritdoc/>
    public IResult<TResult, T> Map<TResult>(Func<T, TResult> map) where TResult : notnull
        => IsOk
            ? Result<TResult, T>.Ok(map(Value!))
            : Result<TResult, T>.Err(Error!);

    /// <inheritdoc/>
    public IResult<T, F> MapErr<F>(Func<T, F> map) where F : notnull
        => IsErr
            ? Result<T, F>.Err(map(Error!))
            : Result<T, F>.Ok(Value);

    /// <inheritdoc/>
    public void Match(Action<IResult<T, T>> ok, Action<IResult<T, T>> err)
    {
        if (IsOk)
            ok(this);
        else
            err(this);
    }

    /// <inheritdoc/>
    IResult<T, T> ICloneable<IResult<T, T>>.Clone()
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
                return (Outcome<T>)MemberwiseClone();
        }

        return Ok(newValue);
    }

    /// <summary>
    /// Implicit operator converts <see cref="Outcome{T}"/> to <see cref="Result{T,E}"/>.
    /// </summary>
    public static implicit operator Result<T, T>(Outcome<T> outcome)
        => outcome.IsOk
            ? Result<T, T>.Ok(outcome.Value)
            : Result<T, T>.Err(outcome.Error);

    /// <summary>
    /// Implicit operator converts <see cref="Result{T,E}"/> to <see cref="Outcome{T}"/>.
    /// </summary>
    public static implicit operator Outcome<T>(Result<T, T> result)
        => result.IsOk
            ? Ok(result.Value)
            : Err(result.Error);
}