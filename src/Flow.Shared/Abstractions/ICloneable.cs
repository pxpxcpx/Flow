namespace Flow.Shared.Abstractions;

/// <summary>
/// Represents an object can be cloned.
/// </summary>
/// <typeparam name="T">Type of the cloneable object.</typeparam>
public interface ICloneable<out T>
{
    /// <summary>
    /// Clone the object itself.
    /// </summary>
    /// <returns></returns>
    T Clone();
}