using Flow.SDK.Plugins.Attributes;

namespace Flow.BuiltIn.Nodes;

public partial class Math
{
    [StaticNode("", "")]
    public static int Add(
        [Input("", "")] int a,
        [Input("", "")] int b)
        => a + b;
}