using Flow.Shared.Abstractions;

namespace Flow.SDK.Dependency.Attributes;

/// <summary>
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