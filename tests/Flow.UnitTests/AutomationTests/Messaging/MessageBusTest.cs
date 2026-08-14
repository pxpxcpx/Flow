using Flow.Automation.Messaging.Components;
using Flow.Automation.Messaging.Messages;
using JetBrains.Annotations;

namespace Flow.UnitTests.AutomationTests.Messaging;

[TestClass]
[TestSubject(typeof(MessageBus<>))]
[TestSubject(typeof(TestObserver))]
public class MessageBusTest
{
    private EventMessage _message;
    
    private MessageBus<EventMessage> _bus;
    
    private TestObserver _observer;
    
    private EventMessage _received;

    [TestInitialize]
    public void TestInitialize()
    {
        _bus = new MessageBus<EventMessage>();
        _observer = new TestObserver();
        _message = new EventMessage(null, EventArgs.Empty);
        _bus.Subscribe(_observer);
    }

    [TestMethod]
    public void Responding_OnNextTest()
    {
        _bus.OnNext(_message);
        Assert.IsFalse(_observer.IsCompleted);
        Assert.HasCount(1, _observer.Messages);
    }
    
    [TestMethod]
    public void Responding_OnErrorTest()
    {
        _bus.OnError(new Exception());
        Assert.IsTrue(_observer.IsError);
    }

    [TestMethod]
    public void Responding_OnCompletedTest()
    {
        _bus.OnCompleted();
        Assert.IsTrue(_observer.IsCompleted);
    }

    [TestMethod]
    public void Responding_OnContinuousMessageTest()
    {
        for(var i = 0; i < 100; i++)
            _bus.OnNext(new EventMessage(null, EventArgs.Empty));
        
        Assert.HasCount(100, _observer.Messages);
        Assert.IsFalse(_observer.IsCompleted);
    }
}