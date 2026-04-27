using Flow.SDK.Attributes;

namespace Flow.Tests.GenerationTest;

public partial class StaticTestNode
{
    [StaticNode("Test Node1", "Node for Add method")]
    public static int TestMethod1(
        [Input("", " ")] int a,
        [Input("", " ")] int b)
    {
        
        return a + b;
    }
}