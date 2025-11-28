using Flow.SDK.Plugins.Attributes;

namespace Flow.Tests.GenerationTest;

public partial class StaticTestNode
{
    [StaticNode("Test Node", "Node for Add method")]
    public static int TestMethod(
        [Input("", "")]int a,
        [Input("1", "")]int b1,
        [Input("3", "")]int c)
    {
        return a + c;
    }
}
