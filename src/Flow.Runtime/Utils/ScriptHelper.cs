using Flow.Runtime.Abstractions;
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
        => script.Graph[guid];

    /// <summary>
    /// Get the runtime GUID of the node.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="script">Script containing this node.</param>
    /// <returns></returns>
    public static Guid? GetRuntimeGuid(this INode node, IScript script)
    {
        if (script.Graph.Count == 0 || !script.Graph.ContainsValue(node))
            return null;
        
        return script.Graph
            .Where(i => i.Value == node)
            .Select(x => x.Key)
            .FirstOrDefault();
    }

    /// <summary>
    /// Get the previous node in the process.
    /// </summary>
    /// <param name="currentRuntimeId">Runtime GUID of the current node.</param>
    /// <param name="script">Script containing this node.</param>
    /// <returns></returns>
    public static Guid[]? GetPreviousProgressNodes(this Guid currentRuntimeId, IScript script)
    {
        if (script.Graph.Count == 0 || !script.Graph.ContainsKey(currentRuntimeId))
            return null;

        return script.ProcessConnection
            .Where(x => x.To.Position == currentRuntimeId)
            .Select(x => x.From.Position)
            .ToArray();
    }
    
    /// <summary>
    /// Get the next node in the process.
    /// </summary>
    /// <param name="currentRuntimeId">Runtime GUID of the current node.</param>
    /// <param name="script">Script containing this node.</param>
    /// <returns></returns>
    public static Guid[]? GetNextProgressNodes(this Guid currentRuntimeId, IScript script)
    {
        if (script.Graph.Count == 0 || !script.Graph.ContainsKey(currentRuntimeId))
            return null;
        
        return script.ProcessConnection
            .Where(x => x.From.Position == currentRuntimeId)
            .Select(x => x.To.Position)
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
        if (script.Graph.Count == 0 || !script.Graph.ContainsKey(currentRuntimeId))
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
        if (script.Graph.Count == 0 || !script.Graph.ContainsKey(currentRuntimeId))
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
    public static INode GetEntry(this IScript script)
    {
        var dict = new Dictionary<Guid, int>();
        foreach (var n in script.Graph)
            dict.Add(n.Key, 0);
        
        foreach (var c in script.ProcessConnection)
            dict[c.To.Position]++;
        
        var entryRtId = dict.FirstOrDefault(x => x.Value == 0).Key;
        return GetNode(entryRtId, script);
    }
}