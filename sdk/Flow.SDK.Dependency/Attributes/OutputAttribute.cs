namespace Flow.SDK.Dependency.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public class OutputAttribute(string name, string description) : RecognizableAttribute(name, description);
