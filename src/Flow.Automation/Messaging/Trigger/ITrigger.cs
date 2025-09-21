namespace Flow.Automation.Messaging.Trigger;

/// <summary>
/// A trigger that works with the <see cref="Poller"/>.
/// </summary>
/// <typeparam name="T">Type of the message.</typeparam>
public interface ITrigger<out T> where T : EventArgs
{
    /// <summary>
    /// Check if the trigger is activated.
    /// </summary>
    /// <returns>If not triggered, null; otherwise, the message.</returns>
    T? CheckIfTriggered();
}