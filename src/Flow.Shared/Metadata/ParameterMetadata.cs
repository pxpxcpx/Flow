using Flow.Shared.Abstractions;

namespace Flow.Shared.Metadata;

/// <summary>
/// Represents a parameter or an output in the node.
/// Immutable identifier used within a variable.
/// </summary>
public record ParameterMetadata :IDescribable
{
    /// <summary>
    /// The index of this variable in the node.
    /// </summary>
    public required int Index { get; set; }

    /// <summary>
    /// Name of the variable.
    /// </summary>
    public required string Identifier { get; set; }

    /// <summary>
    /// Description of the variable.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// The type of the value.
    /// </summary>
    public required Type Type { get; init; }

    /// <summary>
    /// Indicates whether this variable is required.
    /// </summary>
    public required bool IsRequired { get; init; }

    /// <summary>
    /// The default value of this variable, can be null.
    /// </summary>
    public object? DefaultValue { get; init; }
}