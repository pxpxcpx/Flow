namespace Flow.SDK.Attributes;

/// <summary>
/// Used to mark a method if it requires an instance.
/// </summary>
/// <example>
/// If the node's original method resides within an instance class
/// and is not a static method:
/// <code>
/// class Utils
/// {
///     // Target method:
///     public void Method(int param)
///     { ... }
/// }
/// </code>
/// Please mark it with <b>"[InstanceRequired]"</b>
/// and follow other specifications for tagging parameters, etc.
/// <code>
/// [NodeClass]
/// class Utils
/// {
///     // Target method:
///     [InstanceRequired]
///     public void Method([Input]int param)
///     { ... }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Method)]
public class InstanceRequiredNodeAttribute(string name, string description, Type instanceType) 
    : DescribableAttribute(name, description)
{
    public Type InstanceType => instanceType;
}