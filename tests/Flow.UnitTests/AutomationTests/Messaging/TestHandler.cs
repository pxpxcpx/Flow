using Flow.Automation.Messaging;

namespace Flow.UnitTests.AutomationTests.Messaging;

internal class TestHandler : IObserver<ListenerEventMessage>
{
    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(ListenerEventMessage value)
    {
        throw new NotImplementedException();
    }
}