using Flow.Shared.Metadata;

namespace Flow.Shared.Enums;

/// <summary>
/// Plugin Dependency Condition Type Enumeration.
/// </summary>
/// <remarks>
/// Use operator "|" to combine more conditions.
/// </remarks>
/// <example>
/// Indicates that both Name and Guid must match: 
/// <c>Name | Guid</c>
/// </example>
[Flags]
public enum DependencyConditionType
{
    /// <summary>
    /// Compare all properties.
    /// </summary>
    All       = 0b_0000_1111,
    
    /// <summary>
    /// Plugin's <see cref="PluginMetadata.Name"/>.
    /// </summary>
    Name      = 0b_0000_0001,
    
    /// <summary>
    /// Plugin's <see cref="PluginMetadata.Guid"/>.
    /// </summary>
    Guid      = 0b_0000_0010,
    
    /// <summary>
    /// Compare plugin's <see cref="PluginMetadata.Version"/> with MinVersion.
    /// </summary>
    Version   = 0b_0000_0100,
    
    /// <summary>
    /// Compare plugin's <see cref="PluginMetadata.Version"/> with MinVersion.
    /// </summary>
    Predicate = 0b_0000_1000,
}