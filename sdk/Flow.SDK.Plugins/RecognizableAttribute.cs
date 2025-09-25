using Flow.SDK.Plugins.Node.Attributes;

namespace Flow.SDK.Plugins;

/// <summary>
/// <see cref="InputAttribute"/>, <see cref="OutputAttribute"/>'s Base Class, 
/// The flag will have no effect. Do not apply it to any properties or fields.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public abstract class RecognizableAttribute : Attribute, IRecognizable
{
    public string Name { get; protected set; } = string.Empty;
    public string Description { get; protected set; } = string.Empty;

    private RecognizableAttribute() { }
    protected RecognizableAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }
}