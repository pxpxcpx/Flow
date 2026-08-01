namespace Flow.Automation.Messaging.Abstractions;

public interface ISubscriptionManaged
{
    void Unsubscribe(IDisposable subscription);
}