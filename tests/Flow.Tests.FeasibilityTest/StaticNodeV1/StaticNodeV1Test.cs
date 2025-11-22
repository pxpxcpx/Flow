using Flow.Tests.FeasibilityTest.StaticNodeV1;

[TestClass]
public class StaticNodeV1Test
{
    [TestMethod]
    [DataRow(1, 1, 2)]
    [DataRow(5, 10, 15)]
    [DataRow(-5, 5, 0)]
    public void AddNodeInvokeTest(int a, int b, int result)
    {
        var addNode = new AddNode();
        
        addNode.SetValue("ParamA", a);
        addNode.SetValue("ParamB", b);
        addNode.Invoke();
        
        Assert.AreEqual(result, addNode.Result);
    }
}