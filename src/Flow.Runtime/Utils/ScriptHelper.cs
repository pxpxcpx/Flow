using Flow.Runtime.Abstractions;
using Flow.Runtime.ContextManager;
using Flow.Runtime.Models;
using Flow.Shared.Abstractions;

namespace Flow.Runtime.Utils;

/// <summary>
/// Utils for <see cref="IScript"/> objects.
/// </summary>
public static class ScriptHelper
{
    /// <summary>
    /// Get the node in its script by its GUID.
    /// </summary>
    /// <param name="guid">Runtime GUID of the node.</param>
    /// <param name="script">Script containing this node.</param>
    /// <returns></returns>
    public static INode GetNode(this Guid guid, IScript script)
        => script.Nodes.FirstOrDefault(n => n.RuntimeId == guid) ?? throw new KeyNotFoundException();

    /// <summary>
    /// Get the runtime GUID of the node.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="script">Script containing this node.</param>
    /// <returns></returns>
    public static Guid? GetRuntimeGuid(this INode node, IScript script)
    {
        if (script.Nodes.Count == 0 || script.Nodes.All(n => n.RuntimeId != node.RuntimeId))
            return null;
        
        return script.Nodes.FirstOrDefault(n => n.RuntimeId == node.RuntimeId)?.RuntimeId ;
    }

    /// <summary>
    /// Get the previous node in the process.
    /// </summary>
    /// <param name="currentRuntimeId">Runtime GUID of the current node.</param>
    /// <param name="script">Script containing this node.</param>
    /// <returns></returns>
    public static Guid[]? GetPreviousProgressNodes(this Guid currentRuntimeId, IScript script)
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
    /// <param name="currentRuntimeId">Runtime GUID of the current node.</param>
    /// <param name="script">Script containing this node.</param>
    /// <param name="index">Process connection index of the <see cref="IControlStatement"/></param>
    /// <returns></returns>
    public static Guid[]? GetNextProgressNodes(this Guid currentRuntimeId, IScript script, int? index = 0)
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
    /// <param name="currentRuntimeId">Runtime GUID of the current node.</param>
    /// <param name="script">Script containing this node.</param>
    /// <returns><see cref="VariablePosition"/></returns>
    public static VariablePosition[]? GetVariableTarget(this Guid currentRuntimeId, IScript script)
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
    /// <param name="currentRuntimeId"></param>
    /// <param name="script"></param>
    /// <returns></returns>
    public static VariablePosition[]? GetVariableSource(this Guid currentRuntimeId, IScript script)
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
    public static INode? FindEntry(this IScript script)
    {
        var dict = new Dictionary<Guid, int>();
        foreach (var n in script.Nodes)
            dict.Add(n.RuntimeId, 0);
        
        foreach (var c in script.ProcessConnection)
            dict[c.To.NodeId]++;
        
        var entryRtId = dict.FirstOrDefault(x => x.Value == 0).Key;
        return GetNode(entryRtId, script);
    }

    public static Guid? GetRelatedContext(this Guid contextRequiredNodeId, IScript script)
    {
        if (script.Nodes.Count == 0 || script.Nodes.All(n => n.RuntimeId != contextRequiredNodeId))
            return null;

        return script.InstanceConnections
            .Where(x => x.Node.NodeId == contextRequiredNodeId)
            .Select(x => x.InstanceId)
            .FirstOrDefault();
    }

    public static Guid[]? GetRelatedNodes(this Guid contextId, IScript script)
    {
        if (script.ContextManager.ContainsKey(contextId))
            return null;
        
        return script.InstanceConnections
            .Where(x=>x.InstanceId == contextId)
            .Select(x => x.InstanceId)
            .ToArray();
    }

    public static ContextItem? GetContextItem(this Guid id, IScript script)
        => script.ContextManager.TryFindContextItem(id);
}