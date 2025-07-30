namespace Flow.API.Node.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class InputAttribute : IoAttributeBase
{
    public InputAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
