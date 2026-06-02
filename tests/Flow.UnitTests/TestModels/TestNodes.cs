using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;
using Flow.Shared.Models;

namespace Flow.UnitTests.TestModels;

internal class TestSyncNode : Node, IExecutableNode
{
    public bool Executed { get; private set; }

    private static readonly ParameterMetadata[] In =
    [
        new()
        {
            Index = 0,
            Name = null!,
            Description = null!,
            Type = null!,
            IsRequired = false
        }
    ];

    private static readonly ParameterMetadata[] Out =
    [
        new()
        {
            Index = 0,
            Name = null!,
            Description = null!,
            Type = null!,
            IsRequired = false
        },
        new()
        {
            Index = 0,
            Name = null!,
            Description = null!,
            Type = null!,
            IsRequired = false
        },
        new()
        {
            Index = 0,
            Name = null!,
            Description = null!,
            Type = null!,
            IsRequired = false
        }
    ];

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