namespace Flow.Shared.Abstractions;

/// <summary>
/// Indicates that this object requires <see cref="IIoService{TInput,TOutput}"/> during operation.
/// Often used in node plugins that require user interaction.
/// </summary>
/// <typeparam name="TIoService"></typeparam>
/// <typeparam name="TInput"></typeparam>
/// <typeparam name="TOutput"></typeparam>
public interface IIoServiceRequired<out TIoService, TInput, TOutput>
    where TIoService: IIoService<TInput, TOutput>
{
    /// <summary>
    /// Io service that the object required. Will be assigned a value during initialization.
    /// </summary>
    TIoService IoService { get; }
}