namespace Flow.SDK.Plugins.Node.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class OutputAttribute : RecognizableAttributeBase
{
    public OutputAttribute(string name, string description)
        : base(name, description) { }
}
