namespace Flow.Shared.Abstractions;

/// <summary>
/// Indicates that a node requires an instance to be executed.
/// </summary>
public interface IInstanceRequired
{
    /// <summary>
    /// Type of <see cref="Instance"/>.
    /// </summary>
    Type InstanceType { get; }
    
    /// <summary>
    /// Required object.
    /// </summary>
    object? Instance { get; set; }
}