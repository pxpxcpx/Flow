namespace Flow.SDK.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public class InputAttribute(string name, string description) : DescribableAttribute(name, description);
