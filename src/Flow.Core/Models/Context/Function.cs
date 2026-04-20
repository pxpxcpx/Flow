using System.Collections.ObjectModel;
using Flow.Core.Abstractions;
using Flow.Core.Models.Nodes;
using Flow.Core.Models.Positioning;
using Flow.Shared.Abstractions;
using Flow.Shared.Enums;
using Flow.Shared.Metadata;
using Flow.Shared.Results;

namespace Flow.Core.Models.Context;

public class Function : IFunction, IDisposable
{
    private bool _disposed;

    /// <inheritdoc />
    public NodeMetadata Metadata { get; }

    /// <inheritdoc />
    public Guid RuntimeId { get; }

    /// <inheritdoc/>
    public bool IsEnabled { get; set; }

    /// <inheritdoc />
    public NodeStatus Status { get; set; }

    /// <inheritdoc />
    public IExecutableNode Entry => _entry; // TODO

    private readonly EntryNode _entry;

    /// <inheritdoc />
    public IExecutableNode Exit => _exit; // TODO

    private readonly ExitNode _exit;

    /// <summary>
    /// Determines whether the function will exit and return immediately
    /// when the exit reached.
    /// </summary>
    public bool ExitImmediately { get; set; } = true;

    private bool _isNodeEdited;
    private readonly Dictionary<Guid, INode> _publicNodes = new();
    private readonly Dictionary<Guid, INode> _nodes = new();
    private readonly ReadOnlyDictionary<Guid, INode> _presetNodes;

    /// <inheritdoc />
    public Dictionary<Guid, INode> Nodes
    {
        get
        {
            UpdateNode();
            return _publicNodes;
        }
    }

    public INode this[Guid runtimeId]
    {
        get => Nodes[runtimeId];
        set => _nodes[runtimeId] = value;
    }

    public Guid? this[INode node]
    {
        get => GetRuntimeGuid(node);
    }

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
        : this(NodeMetadata.Empty)
    {
    }

    public Function(NodeMetadata metadata)
    {
        RuntimeId = Guid.NewGuid();
        Metadata = metadata;

        _entry = new EntryNode(this);
        _exit = new ExitNode(this);

        var p = new Dictionary<Guid, INode>
        {
            { _entry.RuntimeId, _entry },
            { _exit.RuntimeId, _exit },
        };
        _presetNodes = new ReadOnlyDictionary<Guid, INode>(p);
    }

    /// <summary>
    /// Constructor for cloning.
    /// </summary>
    /// <param name="old"></param>
    private Function(Function old)
    {
        RuntimeId = Guid.NewGuid();
        Metadata = old.Metadata;

        // !!!
        if ((old._entry.Clone() as EntryNode) is not { } entry || (old._exit.Clone() as ExitNode) is not { } exit)
            return;
        _entry = entry;
        _exit = exit;
        _entry.Function = this;
        _exit.Function = this;

        var p = new Dictionary<Guid, INode>
        {
            { _entry.RuntimeId, _entry },
            { _exit.RuntimeId, _exit  },
        };
        _presetNodes = new ReadOnlyDictionary<Guid, INode>(p);

        _nodes = new Dictionary<Guid, INode>(old._nodes);
        ProcessConnections = [.. old.ProcessConnections]; // ???
        VariableConnections = [.. old.VariableConnections];
        InstanceConnections = [.. old.InstanceConnections];

        _isNodeEdited = true;
        old._isNodeEdited = true;
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

    public bool AddProcessConnection(Guid sourceId, Guid targetId, int port = 0)
        => AddProcessConnection(new ProcessConnection
        {
            Source = new NodePort(sourceId),
            Target = new NodePort(targetId),
            Port = port
        });

    public bool AddProcessConnection(INode source, INode target, int port = 0)
        => AddProcessConnection(source.RuntimeId, target.RuntimeId, port);

    public bool RemoveProcessConnection(ProcessConnection connection)
        => ProcessConnections.Remove(connection);

    public bool RemoveProcessConnection(Guid sourceId, Guid targetId, int port = 0)
        => RemoveProcessConnection(new ProcessConnection()
        {
            Source = new NodePort(sourceId),
            Target = new NodePort(targetId),
            Port = port
        });

    public bool RemoveProcessConnection(INode source, INode target)
        => RemoveProcessConnection(source.RuntimeId, target.RuntimeId);

    #endregion

    #region Variable Connections

    public bool AddVariableConnection(VariableConnection connection)
    {
        if (!ContainsNode(connection.Source.NodeId) || !ContainsNode(connection.Target.NodeId))
            return false;

        VariableConnections.Add(connection);
        return true;
    }

    public bool AddVariableConnection(Guid sourceId, Guid targetId, Type? sourceType, Type? targetType)
        => AddVariableConnection(new VariableConnection()
        {
            Source = new VariablePort() { NodeId = sourceId },
            SourceType = sourceType ?? typeof(object),
            Target = new VariablePort() { NodeId = targetId },
            TargetType = targetType ?? typeof(object),
        });

    public bool AddVariableConnection(INode source, INode target, Type? sourceType, Type? targetType)
        => AddVariableConnection(source.RuntimeId, target.RuntimeId, sourceType, targetType);

    public bool RemoveVariableConnection(VariableConnection connection)
        => VariableConnections.Remove(connection);

    public bool RemoveVariableConnection(Guid sourceId, Guid targetId, Type? sourceType, Type? targetType)
        => RemoveVariableConnection(new VariableConnection()
        {
            Source = new VariablePort() { NodeId = sourceId },
            SourceType = sourceType ?? typeof(object),
            Target = new VariablePort() { NodeId = targetId },
            TargetType = targetType ?? typeof(object),
        });

    public bool RemoveVariableConnection(INode source, INode target, Type? sourceType, Type? targetType)
        => RemoveVariableConnection(source.RuntimeId, target.RuntimeId, sourceType, targetType);

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
            .Where(x => x.Source.NodeId == currentRuntimeId && x.Port == index)
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
    /// <returns><see cref="VariablePort"/></returns>
    public VariablePort[]? GetVariableTarget(Guid currentRuntimeId)
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
    public VariablePort[]? GetVariableSource(Guid currentRuntimeId)
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

    public virtual INode? Clone()
        => new Function(this);
}