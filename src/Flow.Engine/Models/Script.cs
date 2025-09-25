using Flow.Engine.Abstractions;
using Flow.SDK.Plugins.Node;

namespace Flow.Engine.Models;

public class Script : IScript
{
    public Guid Id { get; init; }
    public INode Entry { get; set; }
    public Dictionary<Guid, INode> Graph { get; } = new();
    public Dictionary<INode, int> ProcessConnectionsGraph { get; } = new();
    public List<INode> Nodes { get; set; }
    public List<IProcessConnection> ProcessConnection { get; set; }
    public List<IVariableConnection> VariableConnections { get; set; }

    public void InitializeGraph()
    {
        // 初始化map
        foreach (var node in Nodes)
        {
            ProcessConnectionsGraph.Add(node, 0);
        }
    }
}