using Flow.Automation.Messaging.Components;
using JetBrains.Annotations;

namespace Flow.UnitTests.AutomationTests.Messaging;

[TestClass]
[TestSubject(typeof(MessageRouter))]
public class MessageRouterTest
{
    private MessageRouter _router;
    
    private TestHandler _handler1 = new();
    private TestHandler _handler2 = new();
    private TestHandler _handler3 = new();
    private TestHandler _handler4 = new();
    private TestHandler _handler5 = new();
    
    private Dictionary<Guid, TestHandler[]> _dict = new();

    [TestInitialize]
    public void TestInitialize()
    {
        _dict = new Dictionary<Guid, TestHandler[]>
        {
            { Guid.Parse("10000000-0000-0000-0000-000000000000"), [_handler1] },
            { Guid.Parse("20000000-0000-0000-0000-000000000000"), [_handler2, _handler3, _handler4] },
            { Guid.Parse("30000000-0000-0000-0000-000000000000"), [_handler4, _handler5] }
        };
        
        _router = new MessageRouter();
    }
}