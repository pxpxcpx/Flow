using Flow.Shared.Abstractions;

namespace Flow.Shared.Metadata;

/// <summary>
/// Represents a node's metadata.
/// </summary>
public record NodeMetadata : IDescribable
{
    /// <summary>
    /// ID of the node itself.
    /// </summary>
    /// <remarks> Pay attention to distinguish from the RUNTIME ID. </remarks>
    public required Guid Id { get; init; } = Guid.NewGuid();
    
    /// <summary>
    /// Name of the node.
    /// </summary>
    public required string Identifier { get; set; }

    /// <summary>
    /// Description of the node.
    /// </summary>
    public required string Description { get; set; }
    
    /// <summary>
    /// Services that node required.
    /// </summary>
    public IEnumerable<Type>? RequiredServices { get; init; }

    public static NodeMetadata Empty => new NodeMetadata()
    {
        Id = Guid.Empty,
        Identifier = string.Empty,
        Description = string.Empty,
    };
}