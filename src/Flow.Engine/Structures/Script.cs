using Flow.Engine.Abstractions;
using Flow.PDK.Node.Internal;

namespace Flow.Engine.Structures;

public class Script : IScript
{
    public Guid Id { get; init; }
    public IInternalNode Entry { get; set; }
    public Dictionary<Guid, IInternalNode> Graph { get; } = new();
    public Dictionary<IInternalNode, int> ProcessConnectionsGraph { get; } = new();
    public IEnumerable<IInternalNode> Nodes { get; set; }
    public IEnumerable<IProcessConnection> ProcessConnection { get; set; }
    public IEnumerable<IVariableConnection> VariableConnections { get; set; }

    public void InitializeGraph()
    {
        // 初始化map
        foreach (var node in Nodes)
        {
            ProcessConnectionsGraph.Add(node, 0);
        }
    }
    
    protected IInternalNode GetEntry()
    {
        var dict = Nodes.ToDictionary(node => node, node => 0);

        InitializeGraph();
        
        // 统计流程节点，如果存在来自非自身的连接则标记
        foreach (var pc in ProcessConnection)
        {
            if(pc.From.Position.Equals(pc.To.Position))
                continue;
            
            var id = pc.To.Position;
            var to = Nodes.FirstOrDefault(node => node.Id == id);
            
            if(to == null)
                continue;
            
            dict[to]++;
        }

        return dict.FirstOrDefault(x => x.Value == 0).Key;
    }
}