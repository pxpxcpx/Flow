using Flow.SDK.Dependency.Attributes;

namespace Flow.BuiltIn.Nodes;

public partial class Math
{
    [StaticNode("Add", "Node for Add method")]
    public static int Add(
        [Input("Number A", "")] int a,
        [Input("Number A", "")] int b)
    {
        return a+b;
    }
}