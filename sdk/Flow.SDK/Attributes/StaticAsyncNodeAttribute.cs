namespace Flow.SDK.Attributes;

/// <summary>
/// Attribute marked to convert an async method to an async Node.
/// </summary>
/// <param name="name"></param>
/// <param name="description"></param>
[AttributeUsage(AttributeTargets.Method)]
public sealed class StaticAsyncNodeAttribute(string name, string description) : RecognizableAttribute(name, description);