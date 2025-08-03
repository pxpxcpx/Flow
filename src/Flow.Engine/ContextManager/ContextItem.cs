namespace Flow.Engine.ContextManager;

/// <summary>
/// An item in the <see cref="ContextManager{TKey}"/>.
/// </summary>
public sealed record ContextItem
{
    /// <summary>Type of the <see cref="Value"/></summary>
    public Type Type { get; set; }

    /// <summary>Data of the item</summary>
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
    
    public void Deconstruct(out Type type, out object value)
    {
        type = Type;
        value = Value;
    }
}