using Flow.Shared.Abstractions;

namespace Flow.Shared.Metadata;

/// <summary>
/// Represents a node's metadata.
/// </summary>
public readonly record struct NodeMetadata() : IRecognizable
{
    /// <summary>
    /// ID of the node itself.
    /// </summary>
    /// <remarks> Pay attention to distinguish from the RUNTIME ID. </remarks>
    public required Guid Id { get; init; } = Guid.NewGuid();
    
    /// <summary>
    /// Name of the node.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Description of the node.
    /// </summary>
    public required string Description { get; init; }
    
    /// <summary>
    /// Services that node required.
    /// </summary>
    public IEnumerable<Type>? RequiredServices { get; init; }

    public static NodeMetadata Empty => new NodeMetadata()
    {
        Id = Guid.Empty,
        Name = string.Empty,
        Description = string.Empty,
    };
}