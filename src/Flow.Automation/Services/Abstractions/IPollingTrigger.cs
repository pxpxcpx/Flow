using Flow.Automation.Messaging.Components;

namespace Flow.Automation.Services.Abstractions;

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