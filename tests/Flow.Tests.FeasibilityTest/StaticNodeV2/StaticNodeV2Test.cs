namespace Flow.Tests.FeasibilityTest.StaticNodeV2;

[TestClass]
public class StaticNodeV2Test
{
    [TestMethod]
    [DataRow(1, 1, 2)]
    [DataRow(5, 10, 15)]
    [DataRow(-5, 5, 0)]
    public void AddNodeExecuteTest(int a, int b, int result)
    {
        
    }
}