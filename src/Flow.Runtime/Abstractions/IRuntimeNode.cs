using Flow.Shared.Abstractions;

namespace Flow.Runtime.Abstractions;

/// <summary>
/// Provides node runtime properties.
/// </summary>
public interface IRuntimeNode : INode
{
    /// <summary>
    /// ID during the runtime.
    /// </summary>
    /// <remarks> Note the distinction from the <see cref="INode.Guid"/> </remarks>
    Guid RuntimeId { get; }
    
    /// <summary>
    /// Node status during the runtime.
    /// </summary>
    NodeStatus Status { get; set; }
}