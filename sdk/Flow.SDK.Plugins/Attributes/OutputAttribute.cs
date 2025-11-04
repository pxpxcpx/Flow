using Flow.Shared.Metadata;

namespace Flow.SDK.Plugins.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public class OutputAttribute(string name, string description) : RecognizableAttribute(name, description);
