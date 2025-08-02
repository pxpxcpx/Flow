namespace Flow.PDK.Context.Attributes;

/// <summary>
/// Tag the required context objects, which will be managed and stored in the ContextManager
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class ContextObject : RecognizableAttributeBase
{
    public ContextObjectCreationMode CreationMode { get; set; }
    
    public bool CanCreateMultiple { get; set; } = false;
    
    public Type Type { get; set; }
    
    public ContextObject(string name, string description, Type type, ContextObjectCreationMode creationMode = ContextObjectCreationMode.NewInstance)
        : base(name, description)
    {
        Type = type;
        CreationMode = creationMode;
    }
}