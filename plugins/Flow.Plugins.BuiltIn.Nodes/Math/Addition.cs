using Flow.SDK.Plugins.Node;
using Flow.SDK.Plugins.Node.Attributes;

namespace Flow.Plugins.BuiltIn.Nodes.Math;

[Node]
public class Addition : INode
{
    public Guid Id { get; init; }
    public string Name => "Addition";
    public string Description => "Adds two integers together.";

    [Input("Operand1", "The first integer to add.")]
    public int Operand1 { get; set; }
    [Input("Operand2", "The second integer to add.")]
    public int Operand2 { get; set; }

    [Output("Result", "The result of the addition.")]
    public int Out { get; private set; }

    public void Execute()
    {
        Out = Operand1 + Operand2;
    }
}
