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
    /// <inheritdoc/>
    public abstract bool IsRequired { get; set; }

    /// <inheritdoc/>
    public abstract DependencyMetadata Target { get; set; }

    /// <inheritdoc/>
    public abstract DependencyMetadata Current { get; set; }

    /// <inheritdoc/>
    public abstract DependencyConditionType ConditionType { get; set; }

    /// <inheritdoc/>
    public abstract Version? MinVersion { get; set; }

    /// <inheritdoc/>
    public abstract Predicate<(DependencyMetadata, DependencyMetadata)>? Condition { get; set; }

    public abstract Dictionary<NodeMetadata, Type> NodeTypes { get; }
    
    /// <inheritdoc/>
    public abstract Task Initialize();

    public virtual bool IsSatisfied()
        => Current.IsSatisfiedBy(Target, ConditionType, Condition, MinVersion);
}