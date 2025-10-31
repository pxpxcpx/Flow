using System.Diagnostics;
using Flow.Shared.Abstractions;

namespace Flow.Runtime.IO;

/// <summary>
/// Provides a service to output a value to the debug io stream.
/// </summary>
public class DebugIoService : IIoService<string, object>
{
    /// <summary>
    /// This method is UNAVAILABLE because input CANNOT be obtained in debug mode.
    /// Please refer to method <see cref="ConsoleIoService.GetInput"/>.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Throws ALL THE TIME.</exception>
    [Obsolete("This method is UNAVAILABLE because input CANNOT be obtained in debug mode.", true)]
    public object GetInput()
        => throw new InvalidOperationException("Debug IO doesn't have any methods to get an input.");

    /// <inheritdoc />
    public void Output(string value)
        => Debug.WriteLine(value);
}