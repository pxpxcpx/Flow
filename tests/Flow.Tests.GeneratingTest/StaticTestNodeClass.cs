using Flow.SDK.Plugins.Attributes;

namespace Flow.Tests.GenerationTest;

public partial class StaticTestNode
{
    [StaticNode("Test Node1", "Node for Add method")]
    public static int TestMethod1(
        [Input("", " ")]int a,
        [Input("", " ")] int b1,
        [Input("", " ")]int c)
    {
        return a + c;
    }
}
