using Flow.Core.Models.Context;
using Flow.Core.Models.Positioning;
using Flow.Shared.Metadata;
using Flow.Shared.Models;
using JetBrains.Annotations;

namespace Flow.UnitTests.CoreTests.Runtime;

[TestClass]
[TestSubject(typeof(Function))]
public class FunctionTest
{
    private static readonly NodeMetadata TestNodeMetadata = new NodeMetadata()
    {
        Id = Guid.NewGuid(),
        Name = "Function",
        Description = "Function for test.",
    };

    private Function _function;

    [TestInitialize]
    public void TestInit()
    {
        _function = new Function(TestNodeMetadata);
    }

    [TestCleanup]
    public void ClassCleanup()
    {
        _function.Dispose();
    }

    [TestMethod]
    public void AddNodeTest()
    {
        var n = new TestNode();
        _function.AddNode(n);

        foreach (var nf in _function.Nodes)
        {
            Console.WriteLine(nf);
        }

        Assert.HasCount(3, _function.Nodes);
    }

    [TestMethod]
    public void AddConnectionTest()
    {
        AddNodeTest();

        _function.AddProcessConnection(new ProcessConnection()
        {
            Source = new NodePort(
                _function[
                    _function.Nodes
                        .Where(x => x.Value.Metadata.Id == Guid.Parse("B522839C-3BA1-4CDD-B128-6BDFE09A1022"))
                        .Select(x => x.Value).FirstOrDefault()!] ?? Guid.Empty),
            Target = new NodePort(
                _function[
                    _function.Nodes
                        .Where(x => x.Value.Metadata.Id == Guid.Parse("ACCE82DB-F41E-4F4C-8F02-4302A047DA3E"))
                        .Select(x => x.Value).FirstOrDefault()!] ?? Guid.Empty)
        });

        Assert.HasCount(1, _function.ProcessConnections);
    }

    [TestMethod]
    public void ReGetNodesTest()
    {
        AddNodeTest();
        Console.WriteLine(_function.Nodes.Count);

        foreach (var nf in _function.Nodes)
        {
            _function.RemoveNode(nf.Value);
        }

        Console.WriteLine(_function.Nodes.Count);

        AddNodeTest();
        Console.WriteLine(_function.Nodes.Count);
        Assert.HasCount(3, _function.Nodes);
    }

    [TestMethod]
    public void GetProcessNextTest()
    {
        AddConnectionTest();

        var sid = _function[
            _function.Nodes.Where(x => x.Value.Metadata.Id == Guid.Parse("B522839C-3BA1-4CDD-B128-6BDFE09A1022"))
                .Select(x => x.Value).FirstOrDefault()!] ?? Guid.Empty;
        var rid = _function[
            _function.Nodes.Where(x => x.Value.Metadata.Id == Guid.Parse("ACCE82DB-F41E-4F4C-8F02-4302A047DA3E"))
                .Select(x => x.Value).FirstOrDefault()!] ?? Guid.Empty;
        var l = _function.GetNextProgressNodes(sid)!;
        var id = l[0];
        
        Assert.AreEqual(rid, id);
    }

    private class TestNode : Node
    {
    }
}