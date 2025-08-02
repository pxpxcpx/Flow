using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow.Engine;

public sealed class ContextManager
{
    /// <summary>
    /// A dictionary that holds context objects categorized by context names and their types.
    /// </summary>
    public Dictionary<string, Dictionary<Type, object>> Contexts { get; } = new();

    public ContextManager(Dictionary<string, Dictionary<Type, object>> contexts)
        => Contexts = contexts ?? throw new ArgumentNullException(nameof(contexts), "Contexts cannot be null.");
    
    public void AddContextObject<T>(string contextName, T contextObject)
    {
        if (!Contexts.TryGetValue(contextName, out Dictionary<Type, object>? value))
        {
            value = new Dictionary<Type, object>();
            Contexts[contextName] = value;
        }

        var type = typeof(T);
        if (contextObject != null && !value.TryAdd(type, contextObject))
        {
            throw new InvalidOperationException($"Context object of type {type.Name} already exists in context '{contextName}'.");
        }
    }
}