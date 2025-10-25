using Flow.Shared.Abstractions;
using Flow.Shared.Metadata;

namespace Flow.Runtime.Abstractions;

/// <summary>
/// Provides node runtime properties.
/// </summary>
public interface IRuntimeNode : INode
{
    /// <summary>
    /// ID during the runtime.
    /// </summary>
    /// <remarks> Note the distinction from the <see cref="NodeMetadata.Id"/> </remarks>
    Guid RuntimeId { get; }
    
    /// <summary>
    /// Node status during the runtime.
    /// </summary>
    NodeStatus Status { get; set; }
}