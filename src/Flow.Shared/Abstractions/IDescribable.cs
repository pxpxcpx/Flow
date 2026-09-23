namespace Flow.Shared.Abstractions;

/// <summary>
/// Represents an identifier's interface. Immutable identifier used within a variable.
/// Used for object representation or for metadata.
/// </summary>
public interface IDescribable : IRecognizable<string>
{
    /// <summary>
    /// Description of the IDescribable object
    /// </summary>
    string Description { get; set; }
}