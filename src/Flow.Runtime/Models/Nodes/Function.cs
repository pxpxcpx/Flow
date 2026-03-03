using System.Collections.ObjectModel;
using Flow.Runtime.Abstractions;
using Flow.Runtime.Models.Positioning;
using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;
using Flow.Shared.Results;

namespace Flow.Runtime.Models.Nodes;

public class Function : IFunction, IDisposable
{
    private bool _disposed;

    /// <inheritdoc />
    public NodeMetadata Metadata { get; }

    /// <inheritdoc />
    public Guid RuntimeId { get; }

    /// <inheritdoc />
    public NodeStatus Status { get; set; }

    /// <inheritdoc />
    public IExecutableNode Entrance => _entrance; // TODO

    private readonly EntranceNode _entrance;

    /// <inheritdoc />
    public IExecutableNode Exit => _exit; // TODO

    private readonly ExitNode _exit;

    /// <inheritdoc />
    public Dictionary<Guid, INode> Nodes
    {
        get
        {
            UpdateNode();
            return _publicNodes;
        }
    }

    private bool _isNodeEdited;
    private readonly Dictionary<Guid, INode> _publicNodes = new();
    private readonly Dictionary<Guid, INode> _nodes = new();
    private readonly ReadOnlyDictionary<Guid, INode> _presetNodes;

    /// <inheritdoc />
    public HashSet<ProcessConnection> ProcessConnections { get; set; } = new();

    /// <inheritdoc />
    public HashSet<VariableConnection> VariableConnections { get; set; } = new();

    /// <inheritdoc />
    public HashSet<InstanceConnection> InstanceConnections { get; set; } = new();

    /// <inheritdoc />
    public ParameterMetadata[]? InputVariableMetadata { get; } = [];

    /// <inheritdoc />
    public ParameterMetadata[]? OutputVariableMetadata { get; } = [];

    /// <inheritdoc />
    public object?[]? Inputs { get; init; }

    /// <inheritdoc />
    public object?[]? Outputs { get; init; }

    /// <inheritdoc />
    public Result? Result { get; }

    protected Function()
    {
    }

    public Function(NodeMetadata metadata)
    {
        Metadata = metadata;

        _entrance = new EntranceNode(this);
        _exit = new ExitNode(this);

        var p = new Dictionary<Guid, INode>
        {
            { _entrance.RuntimeId, _entrance },
            { _exit.RuntimeId, _entrance },
        };
        _presetNodes = new ReadOnlyDictionary<Guid, INode>(p);
    }

    #region Node

    /// <summary>
    /// Add node to the script.
    /// </summary>
    /// <param name="node"></param>
    public bool AddNode(INode node)
        => TryAdd(node);

    /// <summary>
    /// Update collection <see cref="Nodes"/> when get.
    /// </summary>
    private void UpdateNode()
    {
        if (!_isNodeEdited) return;

        _publicNodes.Clear();

        foreach (var p in _nodes)
        {
            _publicNodes.Add(p.Key, p.Value);
        }

        foreach (var p in _presetNodes)
        {
            _publicNodes.TryAdd(p.Key, p.Value);
        }

        _isNodeEdited = false;
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
        if (!Remove(node.RuntimeId))
            return false;

        ProcessConnections.RemoveWhere(pc => pc.Source.NodeId == node.RuntimeId || pc.Target.NodeId == node.RuntimeId);
        VariableConnections.RemoveWhere(vc => vc.Source.NodeId == node.RuntimeId || vc.Target.NodeId == node.RuntimeId);
        InstanceConnections.RemoveWhere(ic => ic.Node.NodeId == node.RuntimeId);

        return Remove(node.RuntimeId);
    }

    public bool ContainsNode(INode node)
        => ContainsNode(node.RuntimeId);

    public bool ContainsNode(Guid nodeId)
        => Nodes.ContainsKey(nodeId);

    public Guid[]? GetRelatedNodes(Guid contextId)
    {
        return InstanceConnections
            .Where(x => x.InstanceId == contextId)
            .Select(x => x.InstanceId)
            .ToArray();
    }

    #region Internal Node Dict Utils

    private bool TryAdd(INode node)
    {
        _isNodeEdited = true;
        return _nodes.TryAdd(node.RuntimeId, node);
    }

    private bool Remove(Guid key)
    {
        _isNodeEdited = true;
        return _nodes.Remove(key);
    }

    #endregion

    #endregion

    #region Process Connections

    public bool AddProcessConnection(ProcessConnection connection)
    {
        if (!ContainsNode(connection.Source.NodeId) || !ContainsNode(connection.Target.NodeId))
            return false;

        ProcessConnections.Add(connection);
        return true;
    }

    public bool RemoveProcessConnection(ProcessConnection connection)
        => ProcessConnections.Remove(connection);

    #endregion

    #region Variable Connections

    public bool AddVariableConnection(VariableConnection connection)
    {
        if (!ContainsNode(connection.Source.NodeId) || !ContainsNode(connection.Target.NodeId))
            return false;

        VariableConnections.Add(connection);
        return true;
    }

    public bool RemoveVariableConnection(VariableConnection connection)
        => VariableConnections.Remove(connection);

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
            .Where(x => x.Target.NodeId == currentRuntimeId)
            .Select(x => x.Source.NodeId)
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
            .Where(x => x.Source.NodeId == currentRuntimeId && x.Source.Index == index)
            .Select(x => x.Target.NodeId)
            .ToArray();
    }

    /// <summary>
    /// Get variable connections related to value sources.
    /// </summary>
    /// <param name="nodeId"></param>
    /// <returns></returns>
    public VariableConnection[]? GetSourceVariableConnections(Guid nodeId)
    {
        if (Nodes.Count == 0 || Nodes.All(n => n.Key != nodeId))
            return null;

        return VariableConnections
            .Where(c => c.Target.NodeId == nodeId)
            .ToArray();
    }

    /// <summary>
    /// Get variable connections related to value target.
    /// </summary>
    /// <param name="nodeId"></param>
    /// <returns></returns>
    public VariableConnection[]? GetTargetVariableConnections(Guid nodeId)
    {
        if (Nodes.Count == 0 || Nodes.All(n => n.Key != nodeId))
            return null;

        return VariableConnections
            .Where(c => c.Target.NodeId == nodeId)
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
            .Where(x => x.Source.NodeId == currentRuntimeId)
            .Select(x => x.Target)
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
            .Where(x => x.Target.NodeId == currentRuntimeId)
            .Select(x => x.Source)
            .ToArray();
    }

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


    public Guid? GetRelatedContext(Guid contextRequiredNodeId)
    {
        if (Nodes.Count == 0 || Nodes.All(n => n.Key != contextRequiredNodeId))
            return null;

        return InstanceConnections
            .Where(x => x.Node.NodeId == contextRequiredNodeId)
            .Select(x => x.InstanceId)
            .FirstOrDefault();
    }

    #endregion

    public void AddInput(ParameterMetadata parameterMetadata)
    {
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
            // Node (if implements IDisposable)
            foreach (var node in Nodes.OfType<KeyValuePair<Guid, IDisposable>>())
            {
                node.Value.Dispose();
            }
        }

        _disposed = true;
    }
}