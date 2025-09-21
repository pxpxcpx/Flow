namespace Flow.Automation.Messaging.Records;

/// <summary>
/// Represents a container for an event's sender and arguments.
/// </summary>
/// <param name="Sender">The source of the event.</param>
/// <param name="EventArgs">An object that contains the event data.</param>
/// <typeparam name="TSender">Type of the event sender.</typeparam>
/// <typeparam name="TEventArgs">Type of the event arguments.</typeparam>
public record EventMessage<TSender, TEventArgs>(TSender? Sender, TEventArgs EventArgs)
    where TEventArgs : EventArgs
    where TSender : class
{
    public readonly TSender? Sender = Sender;
    public readonly TEventArgs EventArgs = EventArgs;
}

/// <summary>
/// Represents a container for an event's sender and arguments.
/// </summary>
/// <param name="Sender">The source of the event.</param>
/// <param name="EventArgs">An object that contains the event data.</param>
/// <typeparam name="TEventArgs">Type of the event arguments.</typeparam>
public record EventMessage<TEventArgs>(object? Sender, TEventArgs EventArgs)
    : EventMessage<object, TEventArgs>(Sender, EventArgs)
    where TEventArgs : EventArgs;

/// <summary>
/// Represents a container for an event's sender and arguments.
/// </summary>
/// <param name="Sender">The source of the event.</param>
/// <param name="EventArgs">An object that contains the event data.</param>
public record EventMessage(object? Sender, EventArgs EventArgs)
    : EventMessage<object, EventArgs>(Sender, EventArgs)
{
    /// <summary>
    /// Indicates an empty event message with no sender and empty event arguments.
    /// </summary>
    public static readonly EventMessage Empty = new EventMessage(null, EventArgs.Empty);
}
