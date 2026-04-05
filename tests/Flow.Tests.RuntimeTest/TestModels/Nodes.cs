using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Models;

namespace Flow.Tests.RuntimeTest.TestModels;

internal class TestSyncNode : Node, IExecutableNode
{
    public bool Executed { get; private set; }
    
    // public new Guid RuntimeId => Guid.Parse("00000000-0001-0000-0000-000000000000"); 

    public void Execute()
    {
        Console.WriteLine("{0} node executed.", RuntimeId);
        Executed = true;
        Status |= NodeStatus.Completed;
    }
}

internal class TestAsyncNode : Node, IAsyncExecutableNode
{
    public bool Executed { get; private set; }
    
    private Random _random = new(); 

    public async Task ExecuteAsync()
    {
        await Task.Delay(50);
        Console.WriteLine("{0} async node executed.", RuntimeId);
        Executed = true;
        Status |= NodeStatus.Completed;
    }
}

internal class TestDelegateNode : Node, IExecutableNode
{
    public required Action<TestDelegateNode> Action { get; init; }
    
    public void Execute()
    {
        Action.Invoke(this);
    }
}