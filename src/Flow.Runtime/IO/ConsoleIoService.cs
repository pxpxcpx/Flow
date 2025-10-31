using Flow.Shared.Abstractions;

namespace Flow.Runtime.IO;

/// <summary>
/// Provides an io service of the runtime console.
/// </summary>
/// <remarks>
/// Note that this is not the "Input and Output" window displayed in the client,
/// but rather the io in the console during program execution.
/// </remarks>
public class ConsoleIoService : IIoService<string, string?>
{
    /// <inheritdoc />
    public string? GetInput()
        => Console.ReadLine();

    /// <inheritdoc />
    public void Output(string value)
        => Console.WriteLine(value);
}