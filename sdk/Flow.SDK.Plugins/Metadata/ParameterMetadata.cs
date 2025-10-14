namespace Flow.SDK.Plugins.Metadata;

/// <summary>
/// Represents a parameter or an output in the node.
/// Immutable identifier used within a variable.
/// </summary>
public readonly record struct ParameterMetadata(
    int Index, string Name, string Description, Type Type, bool IsRequired, object? DefaultValue = null)
    :IRecognizable
{
    /// <summary>
    /// The index of this variable in the node.
    /// </summary>
    public int Index { get; } = Index;

    /// <summary>
    /// Name of the variable.
    /// </summary>
    public string Name { get; } = Name;

    /// <summary>
    /// Description of the variable.
    /// </summary>
    public string Description { get; } = Description;

    /// <summary>
    /// The type of the value.
    /// </summary>
    public Type Type { get; } = Type;

    /// <summary>
    /// Indicates whether this variable is required.
    /// </summary>
    public bool IsRequired { get; } = IsRequired;

    /// <summary>
    /// The default value of this variable, can be null.
    /// </summary>
    public object? DefaultValue { get; } = DefaultValue;
}