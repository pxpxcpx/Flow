using Flow.Automation.Decisioning.Abstractions;

namespace Flow.Automation.Decisioning.Specifications;

/// <summary>
/// Specifications represents <see cref="bool"/> values: true or false.
/// </summary>
public static class BooleanSpecification
{
    /// <inheritdoc cref="TrueSpecification"/>
    public static readonly TrueSpecification True = new();
    
    /// <inheritdoc cref="FalseSpecification"/>
    public static readonly FalseSpecification False = new();
    
    /// <summary>
    /// Always returns true.
    /// </summary>
    public sealed class TrueSpecification : ISpecification
    {
        public bool Result => true;
        
        internal TrueSpecification() { }
    }

    /// <summary>
    /// Always returns false.
    /// </summary>
    public sealed class FalseSpecification : ISpecification
    {
        public bool Result => false;
        
        internal FalseSpecification() { }
    }
}