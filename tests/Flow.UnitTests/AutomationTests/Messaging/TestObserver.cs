using Flow.Automation.Messaging.Messages;

namespace Flow.UnitTests.AutomationTests.Messaging;

internal class TestObserver : IObserver<EventMessage>
{
    internal List<EventMessage> Messages { get; set; } = [];

    internal bool IsCompleted { get; set; }

    internal bool IsError { get; set; }

    internal Exception Error { get; set; }

    public void OnCompleted()
    {
        IsCompleted = true;
    }

    public void OnError(Exception error)
    {
        Error = error;
        IsError = true;
        IsCompleted = true;
    }

    public void OnNext(EventMessage value)
    {
        Messages.Add(value);
    }
}