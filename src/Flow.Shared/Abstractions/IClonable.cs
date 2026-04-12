namespace Flow.Shared.Abstractions;

/// <summary>
/// Represents an object can be cloned.
/// </summary>
/// <typeparam name="T">Type of the clonable object.</typeparam>
public interface IClonable<out T>
{
    /// <summary>
    /// Clone the object itself.
    /// </summary>
    /// <returns></returns>
    T? Clone();
}