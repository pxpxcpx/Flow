using Flow.SDK.Attributes;

namespace Flow.Tests.FeasibilityTest.StaticNodeV2;

public partial class StaticNodeExampleV2
{
    [StaticNode(name: "Add", description: "Return the sum of two numbers.")]
    public static int Add(
        [Input(name: "Number A", description: "First number to add.")]int a,
        [Input(name: "Number B", description: "Second Number to add.")]int b)
    {
        return a + b;
    }
}