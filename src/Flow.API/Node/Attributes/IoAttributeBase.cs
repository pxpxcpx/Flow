namespace Flow.API.Node.Attributes;

/// <summary>
/// <see cref="InputAttribute"/>, <see cref="OutputAttribute"/>'s Base Class, 
/// The flag will have no effect. Do not apply it to any properties or fields.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class IoAttributeBase : Attribute, IDescribable
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
}