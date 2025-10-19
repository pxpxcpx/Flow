using Flow.SDK.Plugins.Metadata;
using Flow.SDK.Plugins.Node;

namespace Flow.Runtime.Utils;

public static class NodeHelper
{
    private static bool IsNullOrIndexOutOfRange<T>(this T?[]? array, int index)
        => array == null || index < 0 || index >= array.Length;
    
    public static object? GetInput(this INode node, int index)
        => node.Inputs.IsNullOrIndexOutOfRange(index) ? null : node.Inputs![index];

    public static object? GetOutput(this INode node, int index)
        => node.Outputs.IsNullOrIndexOutOfRange(index) ? null : node.Outputs![index];

    public static T? GetInput<T>(this INode node, int index)
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Pass a parameter to the input of the node.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="index">Input parameter's index.</param>
    /// <param name="value">Value to pass.</param>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <returns>True if successful, otherwise, false.</returns>
    public static bool Assign<TValue>(this INode node, int index, TValue? value)
    {
        if (node.InputVariableMetadata.IsNullOrIndexOutOfRange(index) || node.Inputs.IsNullOrIndexOutOfRange(index))
            return false;

        var md = node.InputVariableMetadata![index];
        var type = md.Type;
        if (typeof(TValue) != type)
            return false;
        
        node.Inputs![index] = value;
        return true;
    }

    /// <summary>
    /// Pass a parameter to the input of the node.
    /// </summary>
    /// <remarks>Will NOT check the type of the value.</remarks>
    /// <param name="node"></param>
    /// <param name="index">Input parameter's index.</param>
    /// <param name="value">Value to pass.</param>
    /// <returns>True if successful, otherwise, false.</returns>
    public static bool Assign(this INode node, int index, object? value)
    {
        if (node.InputVariableMetadata.IsNullOrIndexOutOfRange(index) || node.Inputs.IsNullOrIndexOutOfRange(index))
            return false;
        
        node.Inputs![index] = value;
        return true;
    }

    public static bool SetToDefaultValue(this INode node, int index)
    {
        if (node.InputVariableMetadata.IsNullOrIndexOutOfRange(index) || node.Inputs.IsNullOrIndexOutOfRange(index))
            return false;
        
        var md = node.InputVariableMetadata![index];
        if (md.DefaultValue is not { } dv)
            return false;
        
        node.Inputs![index] = dv;
        return true;
    }

    public static IEnumerable<ParameterMetadata> GetUnfilledRequiredValues(this INode node)
    {
        if (node.InputVariableMetadata is null || node.Inputs is null)
            yield break;
        
        for (var i = 0; i < node.InputVariableMetadata.Length; i++)
        {
            var metadata = node.InputVariableMetadata[i];
            
            if (!metadata.IsRequired) 
                continue;
            
            if (node.Inputs[i] is null && metadata.DefaultValue is null)
                yield return metadata;
        }
    }
}