using Flow.Shared.Abstractions;

namespace Flow.Shared.Metadata;

/// <summary>
/// Represents a node's metadata.
/// </summary>
public readonly record struct NodeMetadata(
    string Name, string Description, ParameterMetadata[]? Inputs, ParameterMetadata[]? Outputs)
    : IRecognizable
{
    /// <summary>
    /// ID of the node itself.
    /// </summary>
    /// <remarks> Pay attention to distinguish from the RUNTIME ID. </remarks>
    public required Guid Id { get; init; } = Guid.NewGuid();
    
    /// <summary>
    /// Name of the node.
    /// </summary>
    public required string Name { get; init; } = Name;

    /// <summary>
    /// Description of the node.
    /// </summary>
    public required string Description { get; init; } = Description;

    public Type? InstanceType { get; init; } = null;

    /// <summary>
    /// Metadata of the input parameters.
    /// </summary>
    public required ParameterMetadata[]? Inputs { get; init; } = Inputs;

    /// <summary>
    /// Metadata of the output results.
    /// </summary>
    public required ParameterMetadata[]? Outputs { get; init; } = Outputs;
}