namespace Flow.Automation.Messaging.Records;

public record EventMessage<TSender, TEventArgs>(TSender? Sender, TEventArgs EventArgs)
    where TEventArgs : EventArgs
    where TSender : class
{
    public readonly TSender? Sender = Sender;
    public readonly TEventArgs EventArgs = EventArgs;
}

public record EventMessage<TEventArgs>(object? Sender, TEventArgs EventArgs)
    : EventMessage<object, TEventArgs>(Sender, EventArgs)
    where TEventArgs : EventArgs;

public record EventMessage(object? Sender, EventArgs EventArgs) 
    : EventMessage<object, EventArgs>(Sender, EventArgs);
