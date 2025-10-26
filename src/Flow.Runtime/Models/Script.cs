using Flow.Runtime.Abstractions;

namespace Flow.Runtime.Models;

public class Script : IScript
{
    public Guid RuntimeId { get; init; }
    public IRuntimeNode? Entry { get; set; }
    public Dictionary<Guid, IRuntimeNode> Graph { get; } = new();
    public Dictionary<IRuntimeNode, int> ProcessConnectionsGraph { get; } = new();
    public List<IRuntimeNode> Nodes { get; }
    public List<IProcessConnection> ProcessConnection { get; set; }
    public List<IVariableConnection> VariableConnections { get; set; }

    public void InitializeGraph()
    {
        // 初始化map
        foreach (var node in Graph.Values)
        {
            ProcessConnectionsGraph.Add(node, 0);
        }
    }
}