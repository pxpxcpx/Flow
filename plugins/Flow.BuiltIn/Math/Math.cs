using Flow.SDK.Attributes;

namespace Flow.BuiltIn.Math;

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