using Flow.Automation.Services.Listeners;

namespace Flow.Automation.Messaging;

/// <summary>
/// EventArg included <see cref="EventId"/>, <see cref="ListenerId"/> and <see cref="ConditionId"/>.
/// </summary>
public abstract class ListenerTriggeredEventArgs : EventArgs
{
    /// <summary>
    /// ID of the event.
    /// Generated when a listener creates an event.
    /// </summary>
    public Guid EventId { get; set; }
    
    /// <summary>
    /// Listener or the sender ID.
    /// </summary>
    public Guid ListenerId { get; set; }
    
    /// <summary>
    /// Triggered condition's ID.
    /// Used by <see cref="MessageRouter{THandler}"/> to call the handler.
    /// </summary>
    public Guid ConditionId { get; set; }
}

/// <summary>
/// Message for <c>Listener</c> to generate and will be handled with <c>IHandler</c>.
/// </summary>
/// <param name="Sender"></param>
/// <param name="EventArgs"></param>
public record ListenerEventMessage(IListener Sender, ListenerTriggeredEventArgs EventArgs) 
    : EventMessage<IListener, ListenerTriggeredEventArgs>
    (Sender, EventArgs);
    