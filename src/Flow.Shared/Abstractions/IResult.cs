using Flow.Shared.Results;

namespace Flow.Shared.Abstractions;

// This class was inspired by project "RustSharp",
// Learn more: https://github.com/SlimeNull/RustSharp

/// <summary>
/// Represent the result of a method's execution.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="E"></typeparam>
public interface IResult<T, E> : ICloneable<IResult<T, E>>
    where T : notnull
    where E : notnull
{
    /// <summary>
    /// Indicates whether the operation was successfully executed or not.
    /// </summary>
    bool IsOk { get; }

    /// <summary>
    /// Indicates whether the operation was failed or successfully executed.
    /// </summary>
    bool IsErr { get; }

    /// <summary>
    /// Storage the return value if the operation executed successfully.
    /// </summary>
    T? Value { get; }

    /// <summary>
    /// Storage the error info during the failed execution.
    /// </summary>
    E? Error { get; }

    /// <summary>
    /// Get the <see cref="Value"/> if <see cref="IsOk"/>
    /// </summary>
    /// <returns></returns>
    T Unwrap();

    /// <summary>
    /// Get the <see cref="Error"/> if <see cref="IsErr"/>
    /// </summary>
    /// <returns></returns>
    E UnwrapErr();

    /// <summary>
    /// Process <see cref="Value"/> with function.
    /// </summary>
    /// <param name="map"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    Result<TResult, E> Map<TResult>(Func<T, TResult> map)
        where TResult : notnull;

    /// <summary>
    /// Process <see cref="Error"/> with function.
    /// </summary>
    /// <param name="map"></param>
    /// <typeparam name="F"></typeparam>
    /// <returns></returns>
    Result<T, F> MapErr<F>(Func<E, F> map)
        where F : notnull;

    /// <summary>
    /// Get <see cref="Value"/> and <see cref="Error"/> (if exists)
    /// and process them with two input functions.
    /// </summary>
    /// <param name="ok"></param>
    /// <param name="err"></param>
    void Match(Action<Result<T, E>> ok, Action<Result<T, E>> err);
}