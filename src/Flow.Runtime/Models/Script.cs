using Flow.Runtime.Abstractions;
using Flow.Runtime.ContextManager;
using Flow.Shared.Abstractions;
using Flow.Shared.Metadata;

namespace Flow.Runtime.Models;

public class Script : IScript, IDisposable
{
    private readonly Dictionary<Guid, INode> _nodeLookup = new();
    
    /// <inheritdoc />
    public Guid RuntimeId { get; init; } = Guid.NewGuid();

    /// <inheritdoc />
    public PluginMetadata[] Dependencies { get; init; } = Array.Empty<PluginMetadata>();

    /// <inheritdoc />
    public INode? Entry { get; set; }
    
    /// <inheritdoc />
    public List<INode> Nodes { get; } = new();

    /// <inheritdoc />
    public List<IProcessConnection> ProcessConnections { get; set; } = new();

    /// <inheritdoc />
    public List<IVariableConnection> VariableConnections { get; set; } = new();

    /// <inheritdoc />
    public List<InstanceConnection> InstanceConnections { get; set; } = new();

    /// <inheritdoc />
    public ContextManager<Guid> ContextManager { get; set; } = new();

    public void InitializeGraph()
    {
        throw new NotImplementedException();
    }

    #region CRUD
    
    #region Dependency

    public bool AddDependency()
    {
        throw new NotImplementedException();
    }

    #endregion
    
    #region Node
    
    /// <summary>
    /// Add node to the script.
    /// </summary>
    /// <param name="node"></param>
    public void AddNode(INode node) 
        => Nodes.Add(node);
    
    /// <summary>
    /// Get the node in its script by its GUID.
    /// </summary>
    /// <param name="guid">Runtime GUID of the node.</param>
    /// <returns></returns>
    public INode GetNode(Guid guid)
        => Nodes.FirstOrDefault(n => n.RuntimeId == guid) ?? throw new KeyNotFoundException();
    
    /// <summary>
    /// Get the runtime GUID of the node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public Guid? GetRuntimeGuid(INode node)
    {
        if (Nodes.Count == 0 || Nodes.All(n => n.RuntimeId != node.RuntimeId))
            return null;
        
        return Nodes.FirstOrDefault(n => n.RuntimeId == node.RuntimeId)?.RuntimeId ;
    }

    public bool RemoveNode(INode node)
        => Nodes.Remove(node);

    public bool IsNodeExists(INode node) 
        => Nodes.Any(n => n.RuntimeId == node.RuntimeId);
    
    public bool IsNodeExists(Guid nodeId)
        => Nodes.Any(n => n.RuntimeId == nodeId);

    #endregion
    
    #region Process Connections
    
    public bool AddProcessConnection(IProcessConnection connection)
    {
        if (!IsNodeExists(connection.From.NodeId) || !IsNodeExists(connection.To.NodeId))
            return false;
        
        ProcessConnections.Add(connection);
        return true;
    }

    public bool RemoveProcessConnection(IProcessConnection connection)
        => ProcessConnections.Remove(connection);

    #endregion
    
    #region Variable Connections
    
    public bool AddVariableConnection(IVariableConnection connection)
    {
        if (!IsNodeExists(connection.From.Node) || !IsNodeExists(connection.To.Node))
            return false;
        
        VariableConnections.Add(connection);
        return true;
    }
    
    public bool RemoveVariableConnection(IVariableConnection connection)
        => VariableConnections.Remove(connection);

    #endregion
    
    #region Instance Connections
    
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
    
    #region Context
    
    public ContextItem? GetContextItem(Guid id)
        => ContextManager.TryFindContextItem(id);
    
    #endregion
    
    #endregion
    
    /// <summary>
    /// Get the previous node in the process.
    /// </summary>
    /// <param name="currentRuntimeId">Runtime GUID of the current node.</param>
    /// <returns></returns>
    public Guid[]? GetPreviousProgressNodes(Guid currentRuntimeId)
    {
        if (Nodes.Count == 0 || Nodes.All(n => n.RuntimeId != currentRuntimeId))
            return null;

        return ProcessConnections
            .Where(x => x.To.NodeId == currentRuntimeId)
            .Select(x => x.From.NodeId)
            .ToArray();
    }

    /// <summary>
    /// Get the next node in the process.
    /// </summary>
    /// <param name="currentRuntimeId">Runtime GUID of the current node.</param>
    /// <param name="index">Process connection index of the <see cref="IControlStatement"/></param>
    /// <returns></returns>
    public Guid[]? GetNextProgressNodes(Guid currentRuntimeId, int? index = 0)
    {
        if (Nodes.Count == 0 || Nodes.All(n => n.RuntimeId != currentRuntimeId))
            return null;
        
        return ProcessConnections
            .Where(x => x.From.NodeId == currentRuntimeId && x.From.Index == index)
            .Select(x => x.To.NodeId)
            .ToArray();
    }

    /// <summary>
    /// Get the next variables.
    /// </summary>
    /// <param name="currentRuntimeId">Runtime GUID of the current node.</param>
    /// <returns><see cref="VariablePosition"/></returns>
    public VariablePosition[]? GetVariableTarget(Guid currentRuntimeId)
    {
        if (Nodes.Count == 0 || Nodes.All(n => n.RuntimeId != currentRuntimeId))
            return null;

        return VariableConnections
            .Where(x => x.From.Node == currentRuntimeId)
            .Select(x => x.To)
            .ToArray();
    }

    /// <summary>
    /// Get the source of a variable connection.
    /// </summary>
    /// <param name="currentRuntimeId"></param>
    /// <returns></returns>
    public VariablePosition[]? GetVariableSource(Guid currentRuntimeId)
    {
        if (Nodes.Count == 0 || Nodes.All(n => n.RuntimeId != currentRuntimeId))
            return null;

        return VariableConnections
            .Where(x => x.To.Node == currentRuntimeId)
            .Select(x => x.From)
            .ToArray();
    }

    /// <summary>
    /// Get the entry node of the 
    /// </summary>
    /// <returns></returns>
    public INode? FindEntry()    
    {
        var dict = new Dictionary<Guid, int>();
        foreach (var n in Nodes)
            dict.Add(n.RuntimeId, 0);
        
        foreach (var c in ProcessConnections)
            dict[c.To.NodeId]++;
        
        var entryRtId = dict.FirstOrDefault(x => x.Value == 0).Key;
        return GetNode(entryRtId);
    }

    public Guid? GetRelatedContext(Guid contextRequiredNodeId)
    {
        if (Nodes.Count == 0 || Nodes.All(n => n.RuntimeId != contextRequiredNodeId))
            return null;

        return InstanceConnections
            .Where(x => x.Node.NodeId == contextRequiredNodeId)
            .Select(x => x.InstanceId)
            .FirstOrDefault();
    }

    public Guid[]? GetRelatedNodes(Guid contextId)
    {
        if (ContextManager.ContainsKey(contextId))
            return null;
        
        return InstanceConnections
            .Where(x=>x.InstanceId == contextId)
            .Select(x => x.InstanceId)
            .ToArray();
    }

    public void Dispose()
    {
        // TODO 在此释放托管资源
    }
}