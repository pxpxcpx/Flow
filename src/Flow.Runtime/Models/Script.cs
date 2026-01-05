using Flow.Runtime.Abstractions;
using Flow.Runtime.ContextManager;
using Flow.Shared.Abstractions;
using Flow.Shared.Metadata;

namespace Flow.Runtime.Models;

/// <summary>
/// Store executable script.
/// </summary>
public class Script : IScript, IDisposable
{
    private bool _disposed;

    /// <inheritdoc />
    public Guid RuntimeId { get; init; } = Guid.NewGuid();

    /// <inheritdoc />
    public PluginMetadata[] Dependencies { get; init; } = Array.Empty<PluginMetadata>();

    /// <inheritdoc />
    public INode? Entry { get; set; }

    /// <inheritdoc />
    public Dictionary<Guid, INode> Nodes { get; } = new();

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
        // Entry = Nodes.FirstOrDefault();
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
    {
        Nodes.Add(node.RuntimeId, node);
    }

    /// <summary>
    /// Get the node in its script by its GUID.
    /// </summary>
    /// <param name="guid">Runtime GUID of the node.</param>
    /// <returns></returns>
    public INode GetNode(Guid guid) 
        => !Nodes.TryGetValue(guid, out var node) ? throw new KeyNotFoundException() : node;

    /// <summary>
    /// Get the runtime GUID of the node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public Guid? GetRuntimeGuid(INode node)
    {
        if (Nodes.Count == 0 || Nodes.All(n => n.Key != node.RuntimeId))
            return null;

        return Nodes.FirstOrDefault(n => n.Key == node.RuntimeId).Key;
    }

    /// <summary>
    /// Remove the node and its connections safely.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public bool RemoveNode(INode node)
    {
        if (!Nodes.Remove(node.RuntimeId))
            return false;

        ProcessConnections.RemoveAll(pc => pc.From.NodeId == node.RuntimeId || pc.To.NodeId == node.RuntimeId);
        VariableConnections.RemoveAll(vc => vc.From.NodeId == node.RuntimeId || vc.To.NodeId == node.RuntimeId);
        InstanceConnections.RemoveAll(ic => ic.Node.NodeId == node.RuntimeId);

        return Nodes.Remove(node.RuntimeId);
    }

    public bool ContainsNode(INode node)
        => ContainsNode(node.RuntimeId);

    public bool ContainsNode(Guid nodeId)
        => Nodes.ContainsKey(nodeId);

    #endregion

    #region Process Connections

    public bool AddProcessConnection(IProcessConnection connection)
    {
        if (!ContainsNode(connection.From.NodeId) || !ContainsNode(connection.To.NodeId))
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
        if (!ContainsNode(connection.From.NodeId) || !ContainsNode(connection.To.NodeId))
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
        if (!ContainsNode(connection.Node.NodeId) || !ContainsNode(connection.Node.NodeId))
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
        if (Nodes.Count == 0 || Nodes.All(n => n.Key != currentRuntimeId))
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
        if (Nodes.Count == 0 || Nodes.All(n => n.Key != currentRuntimeId))
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
        if (Nodes.Count == 0 || Nodes.All(n => n.Key != currentRuntimeId))
            return null;

        return VariableConnections
            .Where(x => x.From.NodeId == currentRuntimeId)
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
        if (Nodes.Count == 0 || Nodes.All(n => n.Key != currentRuntimeId))
            return null;

        return VariableConnections
            .Where(x => x.To.NodeId == currentRuntimeId)
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
            dict.Add(n.Key, 0);

        foreach (var c in ProcessConnections)
            dict[c.To.NodeId]++;

        var entryRtId = dict.FirstOrDefault(x => x.Value == 0).Key;
        return GetNode(entryRtId);
    }

    public Guid? GetRelatedContext(Guid contextRequiredNodeId)
    {
        if (Nodes.Count == 0 || Nodes.All(n => n.Key != contextRequiredNodeId))
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
            .Where(x => x.InstanceId == contextId)
            .Select(x => x.InstanceId)
            .ToArray();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            // Managed
            ContextManager.Dispose();

            // Node (if implements IDisposable)
            foreach (var node in Nodes.OfType<KeyValuePair<Guid, IDisposable>>())
            {
                node.Value.Dispose();
            }
        }

        _disposed = true;
    }
}