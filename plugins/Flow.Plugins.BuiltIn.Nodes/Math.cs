using Flow.SDK.Plugins.Node.Attributes;

namespace Flow.Plugins.BuiltIn.Nodes;

public class Math
{
    [StaticNode]
    public static int Add([Input]int a, [Input]int b)
    {
        return a + b;
    }
}