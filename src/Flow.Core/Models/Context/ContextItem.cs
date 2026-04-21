using Flow.Core.Runtime;

namespace Flow.Core.Models.Context;

/// <summary>
/// Storages an instance in a domain.
/// </summary>
/// <example>
/// The following can all be packaged as instances:
/// <code>
/// string, int, double, List, Dictionary, object, etc.
/// </code>
/// However, <b>DO NOT</b> pass an abstract class or interface as a value:
/// <code>
/// ...
/// interface IFoo { ... }
/// class Foo : IFoo { ... }
/// ...
/// var contextItem = new ContextItem(IFoo, Foo);
/// </code>
/// </example>
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
    /// An item in the <see cref="InstanceManager{TKey}"/>.
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