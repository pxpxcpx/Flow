namespace Flow.SDK.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class I18NRequiredAttribute(string identifier) 
    : Attribute
{
    public string Identifier { get; init; } =  identifier;
}