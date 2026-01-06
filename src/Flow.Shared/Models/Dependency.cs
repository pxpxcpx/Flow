using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;
using Flow.Shared.Utils;

namespace Flow.Shared.Models;

/// <summary>
/// Script dependency.
/// </summary>
public record Dependency(
    bool IsRequired,
    PluginMetadata Target,
    PluginMetadata Current,
    DependencyConditionType ConditionType,
    Version? MinVersion,
    Predicate<(PluginMetadata, PluginMetadata)>? Condition = null)
    : IDependency
{
    /// <inheritdoc/>
    public bool IsRequired { get; set; } = IsRequired;

    /// <inheritdoc/>
    public PluginMetadata Target { get; set; } = Target;

    /// <inheritdoc/>
    public PluginMetadata Current { get; set; } = Current;

    /// <inheritdoc/>
    public DependencyConditionType ConditionType { get; set; } = ConditionType;

    /// <inheritdoc/>
    public Version? MinVersion { get; set; } = MinVersion;

    /// <inheritdoc/>
    public Predicate<(PluginMetadata, PluginMetadata)>? Condition { get; set; } = Condition;

    public bool IsSatisfied()
        => Current.IsSatisfiedBy(Target, ConditionType, Condition, MinVersion);
}