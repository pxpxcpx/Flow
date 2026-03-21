namespace Flow.SDK.Dependency.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public class InputAttribute(string name, string description) : RecognizableAttribute(name, description);
