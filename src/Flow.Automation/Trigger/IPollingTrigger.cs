using Flow.Automation.Messaging;

namespace Flow.Automation.Trigger;

/// <summary>
/// A trigger that works with the <see cref="PollingMessageBus"/>.
/// </summary>
/// <typeparam name="T">Type of the message.</typeparam>
public interface IPollingTrigger<out T> where T : EventArgs
{
    /// <summary>
    /// Check if the trigger is activated.
    /// </summary>
    /// <returns>If not triggered, null; otherwise, the message.</returns>
    T? CheckIfTriggered();
}