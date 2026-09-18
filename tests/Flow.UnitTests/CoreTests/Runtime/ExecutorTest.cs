using Flow.Core.Models.Context;
using Flow.Core.Runtime;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;
using JetBrains.Annotations;

namespace Flow.UnitTests.CoreTests.Runtime;

[TestClass]
[TestSubject(typeof(Executor))]
public class SimpleFuncExecutorTests
{
    public TestContext TestContext { get; set; }

    private Function _function = null!;
    private TestSyncNode _syncNode1 = null!;
    private TestSyncNode _syncNode2 = null!;
    private TestAsyncNode _asyncNode1 = null!;

    private static readonly NodeMetadata FunctionMetadata = new()
    {
        Description = "Function for executor unit tests.",
        Id = Guid.Parse("97F542A5-8542-4572-B255-90E90E182E49"),
        Name = "Function",
    };

    [TestInitialize]
    public void Setup()
    {
        _syncNode1 = new TestSyncNode();
        _syncNode2 = new TestSyncNode();
        _asyncNode1 = new TestAsyncNode();
        _function = new Function(FunctionMetadata);

        _function.AddNode(_syncNode1);
        _function.AddNode(_syncNode2);

        _function.AddProcessConnection(_function.Entry, _syncNode1);
        _function.AddProcessConnection(_syncNode1, _syncNode2);
        _function.AddProcessConnection(_syncNode2, _function.Exit);

        Console.WriteLine($"""
                           Entry: {_function.Entry.RuntimeId};
                           #1 : {_syncNode1.RuntimeId};
                           #2 : {_syncNode2.RuntimeId};
                           Exit: {_function.Exit.RuntimeId};

                           """);
    }

    [TestMethod]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange & Act
        var executor = new Executor(_function);

        // Assert
        Assert.AreEqual(ProcessorStatus.Ready, executor.Status);
        Assert.IsNull(executor.Result);
    }

    [TestMethod]
    public async Task Execute_WithSyncNode_ShouldExecuteNodeAndMarkCompleted()
    {
        // Arrange
        var executor = new Executor(_function);

        // Act
        await executor.Execute();

        // Assert
        Assert.IsTrue(_syncNode1.Executed);
        Assert.IsTrue(_syncNode1.Status.HasFlag(NodeStatus.Completed));
    }

    [TestMethod]
    public async Task Execute_WithAsyncNode_ShouldExecuteAsyncNode()
    {
        // Arrange
        var asyncNode = new TestAsyncNode();
        _function.AddNode(asyncNode);
        _function.RemoveProcessConnection(_syncNode2, _function.Exit);
        _function.AddProcessConnection(_syncNode2, asyncNode);
        _function.AddProcessConnection(asyncNode, _function.Exit);

        var executor = new Executor(_function);

        // Act
        await executor.Execute();

        // Assert
        // Wait 100ms cause it is fire and forget.
        // await Task.Delay(50, TestContext.CancellationToken);
        Assert.IsTrue(asyncNode.Executed);
        Assert.IsTrue(asyncNode.Status.HasFlag(NodeStatus.Completed));
    }

    [TestMethod]
    public async Task Execute_WhenExitReached_ShouldCompleteWithoutError()
    {
        // Arrange
        var executor = new Executor(_function);

        // Act
        await executor.Execute();

        // Assert - no exception, executor should finish
        Assert.IsTrue(_syncNode1.Executed);
        Assert.IsTrue(_syncNode2.Executed);
    }

    // TODO
    [TestMethod]
    public async Task Execute_WithSubFunction_ShouldSwitchAndReturn()
    {
        Assert.Inconclusive("Mocking of sub function required.");
    }

    [TestMethod]
    public void Assign_WithLinearNode()
    {
        // Arrange

        // Act

        // Assert
    }

    [TestMethod]
    public void Assign_WithParallelNode_ShouldAssignAllNodes()
    {
        // Arrange

        // Act

        // Assert
    }

    [TestMethod]
    public void Assign_WithMultipleValues()
    {
        // Arrange

        // Act

        // Assert
    }

    [TestMethod]
    public void Assign_WithAsyncNodeAsSource_ShouldWaitAndExecute()
    {
        // Arrange

        // Act

        // Assert
    }

    [TestMethod]
    public void Dispose_ShouldReleaseResources()
    {
        // Arrange
        var executor = new Executor(_function);

        // Act
        executor.Dispose();

        // Assert - no exception, should not throw
        // 实际可验证 SemaphoreSlim 是否释放，但难以直接验证
    }
}

[TestClass]
public class ComplexFuncExecutorTests
{
    public TestContext TestContext { get; set; }

    private Function? _function;
    private TestSyncNode? _syncNode1;
    private TestSyncNode? _syncNode2;
    private TestAsyncNode? _asyncNode1;

    private static readonly NodeMetadata FunctionMetadata = new()
    {
        Description = "Function for executor unit tests.",
        Id = Guid.Parse("E5919D30-5279-4BF7-94D1-9718F91E35E3"),
        Name = "Function",
    };

    [TestInitialize]
    public void Setup()
    {
        _syncNode1 = new TestSyncNode();
        _syncNode2 = new TestSyncNode();
        _asyncNode1 = new TestAsyncNode();
        _function = new Function(FunctionMetadata);

        _function.AddNode(_syncNode1);
        _function.AddNode(_syncNode2);

        _function.AddProcessConnection(_function.Entry, _syncNode1);
        _function.AddProcessConnection(_syncNode1, _syncNode2);
        _function.AddProcessConnection(_syncNode2, _function.Exit);

        Console.WriteLine($"""
                           Entry: {_function.Entry.RuntimeId};
                           #1 : {_syncNode1.RuntimeId};
                           #2 : {_syncNode2.RuntimeId};
                           Exit: {_function.Exit.RuntimeId};

                           """);
    }
}