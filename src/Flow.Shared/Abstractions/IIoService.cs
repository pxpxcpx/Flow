namespace Flow.Shared.Abstractions;

/// <summary>
/// Interface for providing generic I/O service operations.
/// (e.g.) console (flow client), image, runtime cli console, etc.
/// </summary>
public interface IIoService<in TInput, out TOutput>
{
    /// <summary>
    /// Get input in the IO service.
    /// </summary>
    /// <returns>The value get from the io service.</returns>
    TOutput? GetInput();
    
    /// <summary>
    /// Output value in the service.
    /// </summary>
    /// <param name="value">Value to output.</param>
    void Output(TInput value);
}