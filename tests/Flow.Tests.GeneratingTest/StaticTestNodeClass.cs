using Flow.SDK.Plugins.Attributes;

namespace Flow.Tests.GenerationTest;

public partial class StaticTestNode
{
    [StaticNode("Test Node", "Node for Add method")]
    public static int TestMethod(
        [Input("1", "")]int a,
        [Input("2", "")]int b,
        [Input("3", "")]int c)
    {
        return a + c;
    }
}
