namespace Flow.Automation.Decisioning.Abstractions;

/// <summary>
/// Used to form conditional groups and perform logical operations,
/// only caring about the result.
/// </summary>
public interface ICondition
{
    /// <summary>
    /// The result of the instance.
    /// </summary>
    bool Result { get; }
}