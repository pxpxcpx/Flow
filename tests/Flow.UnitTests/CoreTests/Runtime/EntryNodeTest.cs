using Flow.Core.Models.Context;
using Flow.Core.Models.Nodes;
using Flow.Shared.Metadata;
using JetBrains.Annotations;

namespace Flow.UnitTests.CoreTests.Runtime;

[TestClass]
[TestSubject(typeof(EntryNode))]
public class EntryNodeTest
{
    private static NodeMetadata _nodeMetadata = new()
    {
        Identifier = "TestNode",
        Description = "TestNode",
        Id = Guid.Parse("F43D5F2F-61E4-4814-8E68-8A218DF30740"),
    };

    private Function _function;

    private EntryNode _entryNode;

    [TestInitialize]
    public void Initialize()
    {
        _function = new Function(_nodeMetadata);
        _entryNode = new EntryNode(_function);
    }

    [TestMethod]
    public void ExecuteTest()
    {
    }

    [TestMethod]
    public void SyncTest()
    {
        
    }
}