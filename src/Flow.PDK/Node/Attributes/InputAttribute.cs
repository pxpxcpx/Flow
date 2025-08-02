namespace Flow.PDK.Node.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class InputAttribute : RecognizableAttributeBase
{
    public InputAttribute(string name, string description)
        : base(name, description) { }
}
