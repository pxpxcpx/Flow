using Flow.Runtime.Abstractions;
using Flow.Shared.Models;

namespace Flow.Runtime.Models;

/// <summary>
/// Store executable script.
/// </summary>
public class Script : Function, IScript
{
    /// <inheritdoc />
    public Guid RuntimeId { get; init; } = Guid.NewGuid();

    /// <inheritdoc />
    public Dependency[] Dependencies { get; init; } = Array.Empty<Dependency>();

    public void Initialize()
    {
        // Entry = Nodes.FirstOrDefault();
        throw new NotImplementedException();
    }
    
    #region Dependency

    public bool AddDependency()
    {
        throw new NotImplementedException();
    }
    
    public bool RemoveDependency()
    {
        throw new NotImplementedException();
    }

    public bool ContainsDependency(Dependency dependency)
    {
        throw new NotImplementedException();
    }
    
    #endregion
}