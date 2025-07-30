namespace Flow.API;

public interface IDescribable
{
    /// <summary>
    /// Name to the IDescribable object
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Description to the IDescribable object
    /// </summary>
    string Description { get; }
}