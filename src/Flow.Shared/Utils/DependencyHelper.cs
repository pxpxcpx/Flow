using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;

namespace Flow.Shared.Utils;

public static class DependencyHelper
{
    public static bool IsSatisfiedBy(this IDependency target)
        => target.Current.IsSatisfiedBy(target.Target, target.ConditionType, target.Condition, target.MinVersion);

    /// <summary>
    /// Determine whether a plugin (or a dependency) meets the requirements.
    /// </summary>
    /// <param name="metadata">Plugins being compared.</param>
    /// <param name="target">Target plugin</param>
    /// <param name="conditionType"><see cref="DependencyConditionType"/></param>
    /// <param name="condition"><see cref="IDependency.Condition"/>, nullable.</param>
    /// <param name="minVersion"><see cref="IDependency.MinVersion"/>, nullable.</param>
    /// <returns></returns>
    public static bool IsSatisfiedBy(
        this DependencyMetadata metadata, DependencyMetadata target,
        DependencyConditionType conditionType, Predicate<(DependencyMetadata, DependencyMetadata)>? condition, Version? minVersion)
    {
        if (conditionType == DependencyConditionType.All)
            return metadata == target;
        
        var isSatisfied = false;
        
        if (conditionType.HasFlag(DependencyConditionType.Name))
            isSatisfied |= metadata.Name == target.Name;
        if (conditionType.HasFlag(DependencyConditionType.Guid))
            isSatisfied |= metadata.Guid == target.Guid;
        if (conditionType.HasFlag(DependencyConditionType.Version))
            isSatisfied |= metadata.Version >= (minVersion ?? target.Version);
        if (conditionType.HasFlag(DependencyConditionType.Predicate))
            isSatisfied |= condition?.Invoke((metadata, target)) ?? false;

        return isSatisfied;
    }
}