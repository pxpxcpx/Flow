namespace Flow.API.Node.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class OutputAttribute : IoAttributeBase
{
    public OutputAttribute(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
