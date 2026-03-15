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
    DependencyMetadata Target,
    DependencyMetadata Current,
    DependencyConditionType ConditionType,
    Version? MinVersion,
    Predicate<(DependencyMetadata, DependencyMetadata)>? Condition = null)
    : IDependency
{
    /// <inheritdoc/>
    public bool IsRequired { get; set; } = IsRequired;

    /// <inheritdoc/>
    public DependencyMetadata Target { get; set; } = Target;

    /// <inheritdoc/>
    public DependencyMetadata Current { get; set; } = Current;

    /// <inheritdoc/>
    public DependencyConditionType ConditionType { get; set; } = ConditionType;

    /// <inheritdoc/>
    public Version? MinVersion { get; set; } = MinVersion;

    /// <inheritdoc/>
    public Predicate<(DependencyMetadata, DependencyMetadata)>? Condition { get; set; } = Condition;

    public bool IsSatisfied()
        => Current.IsSatisfiedBy(Target, ConditionType, Condition, MinVersion);
}