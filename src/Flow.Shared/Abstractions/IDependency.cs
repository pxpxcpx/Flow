using System;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;

namespace Flow.Shared.Abstractions;

/// <summary>
/// Interface for script dependencies.
/// </summary>
public interface IDependency
{
    /// <summary>
    /// Indicates whether it is required.
    /// </summary>
    bool IsRequired { get; set; }
    
    /// <summary>
    /// Target plugin's metadata.
    /// </summary>
    PluginMetadata Target { get; set; }
    
    /// <summary>
    /// Current (or installed) plugin's metadata.
    /// </summary>
    PluginMetadata Current { get; set; }
    
    /// <summary>
    /// Used as the basis for determining whether the conditions have been met since then.
    /// </summary>
    DependencyConditionType ConditionType { get; set; }
    
    /// <summary>
    /// Min version of the plugin.
    /// </summary>
    Version? MinVersion { get; set; }
    
    /// <summary>
    /// Predicate for evaluating custom conditions.
    /// Triggers only when <see cref="ConditionType"/>
    /// contains <see cref="DependencyConditionType.Predicate"/>.
    /// </summary>
    /// <remarks>
    /// Gives a tuple contains two elements,
    /// follows the format below:
    /// (CurrentPlugin, TargetPlugin)
    /// </remarks>
    Predicate<(PluginMetadata, PluginMetadata)>? Condition { get; set; }
}