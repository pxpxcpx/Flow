using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;
using Flow.Shared.Utils;

namespace Flow.Shared.Models;

/// <summary>
/// Script dependency.
/// </summary>
public abstract class Dependency : IDependency
{
    /// <summary>
    /// Script dependency.
    /// </summary>
    protected Dependency(bool isRequired,
        DependencyMetadata target,
        DependencyMetadata current,
        DependencyConditionType conditionType,
        Version? minVersion,
        Predicate<(DependencyMetadata, DependencyMetadata)>? condition = null)
    {
        IsRequired = isRequired;
        Target = target;
        Current = current;
        ConditionType = conditionType;
        MinVersion = minVersion;
        Condition = condition;
    }

    /// <inheritdoc/>
    public bool IsRequired { get; set; }

    /// <inheritdoc/>
    public DependencyMetadata Target { get; set; }

    /// <inheritdoc/>
    public DependencyMetadata Current { get; set; }

    /// <inheritdoc/>
    public DependencyConditionType ConditionType { get; set; }

    /// <inheritdoc/>
    public Version? MinVersion { get; set; }

    protected static I18NHelper? I18NHelper; 

    /// <inheritdoc/>
    public Predicate<(DependencyMetadata, DependencyMetadata)>? Condition { get; set; }

    public bool IsSatisfied()
        => Current.IsSatisfiedBy(Target, ConditionType, Condition, MinVersion);
}