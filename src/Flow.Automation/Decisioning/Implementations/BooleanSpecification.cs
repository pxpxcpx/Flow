using Flow.Automation.Decisioning.Abstractions;

namespace Flow.Automation.Decisioning.Implementations;

/// <summary>
/// Specifications represents <see cref="bool"/> values: true or false.
/// </summary>
public static class BooleanSpecification
{
    /// <inheritdoc cref="TrueCondition"/>
    public static readonly TrueCondition True = new();
    
    /// <inheritdoc cref="FalseCondition"/>
    public static readonly FalseCondition False = new();
    
    /// <summary>
    /// Always returns true.
    /// </summary>
    public sealed class TrueCondition : ICondition
    {
        public bool Result => true;
        
        internal TrueCondition() { }
    }

    /// <summary>
    /// Always returns false.
    /// </summary>
    public sealed class FalseCondition : ICondition
    {
        public bool Result => false;
        
        internal FalseCondition() { }
    }
}