namespace Flow.SDK.Plugins.Attributes;

[AttributeUsage(AttributeTargets.Method , Inherited = false)]
public class StaticNodeAttribute(string name, string description) : RecognizableAttribute(name, description);