using Flow.SDK.Plugins.Attributes;

namespace Flow.BuiltIn.Nodes;

public class Math
{
    [StaticNode]
    public static int Add([Input]int a, [Input]int b)
    {
        return a + b;
    }
}