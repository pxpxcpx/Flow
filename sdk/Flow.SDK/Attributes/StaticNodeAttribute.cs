namespace Flow.SDK.Attributes;

/// <summary>
/// Attribute marked to convert a method to a node.
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
[AttributeUsage(AttributeTargets.Method)]
public sealed class StaticNodeAttribute(string name, string description) : RecognizableAttribute(name, description);