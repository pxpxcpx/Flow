using Flow.Shared.Abstractions;

namespace Flow.Runtime.ContextManager;

/// <summary>
/// Item in the <see cref="ContextManager{TKey}"/>.
/// </summary>
public sealed record ContextItem
{
    /// <summary>
    /// The name of the context item. 
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Type of the <see cref="Value"/>.
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// Data of the item.
    /// </summary>
    public object Value { get; set; }
    
    /// <summary>
    /// An item in the <see cref="ContextManager{TKey}"/>.
    /// </summary>
    /// <param name="type">type of the <see cref="Value"/></param>
    /// <param name="value">Data of the item</param>
    public ContextItem(Type type, object value)
    {
        Type = type;
        Value = value;
    }
    
    /// <summary>
    /// Get the type and value of context item.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    public void Deconstruct(out Type type, out object value)
    {
        type = Type;
        value = Value;
    }
}