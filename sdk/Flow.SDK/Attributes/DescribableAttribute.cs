using Flow.Shared.Abstractions;

namespace Flow.SDK.Attributes;

/// <summary>
/// The flag will have no effect. Do not apply it to any properties or fields.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public abstract class DescribableAttribute : Attribute, IDescribable
{
    public string Identifier { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    private DescribableAttribute() { }
    protected DescribableAttribute(string name, string description)
    {
        Identifier = name;
        Description = description;
    }
}