using Flow.Shared.Metadata;

namespace Flow.SDK.Plugins.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class InstanceRequiredNodeAttribute(string name, string description, Type instanceType) 
    : RecognizableAttribute(name, description)
{
    public Type InstanceType => instanceType;
}