using Flow.Runtime.Abstractions;
using Flow.Runtime.ContextManager;
using Flow.Shared.Abstractions;
using Flow.Shared.Metadata;

namespace Flow.Runtime.Models;

public class Script : IScript
{
    public Guid RuntimeId { get; init; }
    
    public PluginMetadata[] Dependencies { get; init; }
    
    public INode? Entry { get; set; }
    
    public List<INode> Nodes { get; }
    
    public List<IProcessConnection> ProcessConnection { get; set; }
    
    public List<IVariableConnection> VariableConnections { get; set; }
    
    public List<InstanceConnection> InstanceConnections { get; set; }
    
    public ContextManager<Guid> ContextManager { get; set; }

    public void InitializeGraph()
    {
        throw new NotImplementedException();
    }

    #region CRUD
    
    public bool AddDependency()
    {
        throw new NotImplementedException();
    }

    public void AddNode(INode node) 
        => Nodes.Add(node);

    public bool RemoveNode(INode node)
        => Nodes.Remove(node);

    public bool IsNodeExists(INode node) 
        => Nodes.Any(n => n.RuntimeId == node.RuntimeId);
    
    public bool IsNodeExists(Guid nodeId)
        => Nodes.Any(n => n.RuntimeId == nodeId);

    public bool AddProcessConnection(IProcessConnection connection)
    {
        if (!IsNodeExists(connection.From.NodeId) || !IsNodeExists(connection.To.NodeId))
            return false;
        
        ProcessConnection.Add(connection);
        return true;
    }

    public bool RemoveProcessConnection(IProcessConnection connection)
        => ProcessConnection.Remove(connection);

    public bool AddVariableConnection(IVariableConnection connection)
    {
        if (!IsNodeExists(connection.From.Node) || !IsNodeExists(connection.To.Node))
            return false;
        
        VariableConnections.Add(connection);
        return true;
    }
    
    public bool RemoveVariableConnection(IVariableConnection connection)
        => VariableConnections.Remove(connection);

    public bool AddInstanceConnection(InstanceConnection connection)
    {
        if (!IsNodeExists(connection.Node.NodeId) || !IsNodeExists(connection.Node.NodeId))
            return false;
        
        InstanceConnections.Add(connection);
        return true;
    }

    public bool RemoveInstanceConnection(InstanceConnection connection)
        => InstanceConnections.Remove(connection);
    
    #endregion

    /// <summary>
    /// Get the node in its script by its GUID.
    /// </summary>
    /// <param name="script">Script containing this node.</param>
    /// <param name="guid">Runtime GUID of the node.</param>
    /// <returns></returns>
    public static INode GetNode(IScript script, Guid guid)
        => script.Nodes.FirstOrDefault(n => n.RuntimeId == guid) ?? throw new KeyNotFoundException();

    /// <summary>
    /// Get the runtime GUID of the node.
    /// </summary>
    /// <param name="script">Script containing this node.</param>
    /// <param name="node"></param>
    /// <returns></returns>
    public static Guid? GetRuntimeGuid(IScript script, INode node)
    {
        if (script.Nodes.Count == 0 || script.Nodes.All(n => n.RuntimeId != node.RuntimeId))
            return null;
        
        return script.Nodes.FirstOrDefault(n => n.RuntimeId == node.RuntimeId)?.RuntimeId ;
    }

    /// <summary>
    /// Get the previous node in the process.
    /// </summary>
    /// <param name="script">Script containing this node.</param>
    /// <param name="currentRuntimeId">Runtime GUID of the current node.</param>
    /// <returns></returns>
    public static Guid[]? GetPreviousProgressNodes(IScript script, Guid currentRuntimeId)
    {
        if (script.Nodes.Count == 0 || script.Nodes.All(n => n.RuntimeId != currentRuntimeId))
            return null;

        return script.ProcessConnection
            .Where(x => x.To.NodeId == currentRuntimeId)
            .Select(x => x.From.NodeId)
            .ToArray();
    }

    /// <summary>
    /// Get the next node in the process.
    /// </summary>
    /// <param name="script">Script containing this node.</param>
    /// <param name="currentRuntimeId">Runtime GUID of the current node.</param>
    /// <param name="index">Process connection index of the <see cref="IControlStatement"/></param>
    /// <returns></returns>
    public static Guid[]? GetNextProgressNodes(IScript script, Guid currentRuntimeId, int? index = 0)
    {
        if (script.Nodes.Count == 0 || script.Nodes.All(n => n.RuntimeId != currentRuntimeId))
            return null;
        
        return script.ProcessConnection
            .Where(x => x.From.NodeId == currentRuntimeId && x.From.Index == index)
            .Select(x => x.To.NodeId)
            .ToArray();
    }

    /// <summary>
    /// Get the next variables.
    /// </summary>
    /// <param name="script">Script containing this node.</param>
    /// <param name="currentRuntimeId">Runtime GUID of the current node.</param>
    /// <returns><see cref="VariablePosition"/></returns>
    public static VariablePosition[]? GetVariableTarget(IScript script, Guid currentRuntimeId)
    {
        if (script.Nodes.Count == 0 || script.Nodes.All(n => n.RuntimeId != currentRuntimeId))
            return null;

        return script.VariableConnections
            .Where(x => x.From.Node == currentRuntimeId)
            .Select(x => x.To)
            .ToArray();
    }

    /// <summary>
    /// Get the source of a variable connection.
    /// </summary>
    /// <param name="script"></param>
    /// <param name="currentRuntimeId"></param>
    /// <returns></returns>
    public static VariablePosition[]? GetVariableSource(IScript script, Guid currentRuntimeId)
    {
        if (script.Nodes.Count == 0 || script.Nodes.All(n => n.RuntimeId != currentRuntimeId))
            return null;

        return script.VariableConnections
            .Where(x => x.To.Node == currentRuntimeId)
            .Select(x => x.From)
            .ToArray();
    }

    /// <summary>
    /// Get the entry node of the script.
    /// </summary>
    /// <param name="script"></param>
    /// <returns></returns>
    public static INode? FindEntry(IScript script)
    {
        var dict = new Dictionary<Guid, int>();
        foreach (var n in script.Nodes)
            dict.Add(n.RuntimeId, 0);
        
        foreach (var c in script.ProcessConnection)
            dict[c.To.NodeId]++;
        
        var entryRtId = dict.FirstOrDefault(x => x.Value == 0).Key;
        return GetNode(script, entryRtId);
    }

    public static Guid? GetRelatedContext(IScript script, Guid contextRequiredNodeId)
    {
        if (script.Nodes.Count == 0 || script.Nodes.All(n => n.RuntimeId != contextRequiredNodeId))
            return null;

        return script.InstanceConnections
            .Where(x => x.Node.NodeId == contextRequiredNodeId)
            .Select(x => x.InstanceId)
            .FirstOrDefault();
    }

    public static Guid[]? GetRelatedNodes(IScript script, Guid contextId)
    {
        if (script.ContextManager.ContainsKey(contextId))
            return null;
        
        return script.InstanceConnections
            .Where(x=>x.InstanceId == contextId)
            .Select(x => x.InstanceId)
            .ToArray();
    }

    public static ContextItem? GetContextItem(IScript script, Guid id)
        => script.ContextManager.TryFindContextItem(id);
}