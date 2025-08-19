namespace Flow.PDK.Context.Enums;

public enum ContextObjectCreationMode
{
    /// <summary>
    /// The object will be created as a new instance each time it is requested.
    /// </summary>
    NewInstance,
    
    /// <summary>
    /// The object will be created as a singleton,
    /// meaning only one instance will exist for the lifetime of the application
    /// unless it is explicitly disposed or manually removed.
    /// </summary>
    Static
}