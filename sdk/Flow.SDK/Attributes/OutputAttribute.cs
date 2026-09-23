namespace Flow.SDK.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public class OutputAttribute(string name, string description) : DescribableAttribute(name, description);
