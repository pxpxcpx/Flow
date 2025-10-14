namespace Flow.SDK.Plugins.Metadata;

/// <summary>
/// Represents an identifier's interface. Immutable identifier used within a variable.
/// Used for object representation or for metadata.
/// </summary>
public interface IRecognizable
{
    /// <summary>
    /// Name to the IRecognizable object
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Description to the IRecognizable object
    /// </summary>
    string Description { get; }
}