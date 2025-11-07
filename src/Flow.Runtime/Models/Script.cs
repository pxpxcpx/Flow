using Flow.Runtime.Abstractions;
using Flow.Shared.Abstractions;
using Flow.Shared.Metadata;

namespace Flow.Runtime.Models;

public class Script : IScript
{
    public Guid RuntimeId { get; init; }
    public PluginMetadata[] Dependencies { get; init; }
    public INode? Entry { get; set; }
    public Dictionary<Guid, INode> Graph { get; } = new();
    public Dictionary<INode, int> ProcessConnectionsGraph { get; } = new();
    public List<INode> Nodes { get; }
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