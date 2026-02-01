namespace Flow.Automation.Decisioning.Abstractions;

/// <summary>
/// Inherits from <see cref="ICondition"/>,
/// add the instance as the data to be evaluated.
/// </summary>
/// <typeparam name="T">Type of the object to be evaluated.</typeparam>
public interface IParamCondition<in T> : ICondition
{
    /// <summary>
    /// Used as a criterion for judgment.
    /// </summary>
    object?[]? Conditions { get; set; }

    /// <summary>
    /// A method for determining whether the object (or data) meets the conditions.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    bool Evaluate(T? obj = default);
    
    /// <summary>
    /// Set <see cref="Conditions"/>
    /// </summary>
    /// <param name="conditions"></param>
    void SetConditions(object?[]? conditions);
}