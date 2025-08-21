using Flow.Automation.Decisioning.Abstractions;

namespace Flow.Automation.Decisioning;

/// <summary>
/// Specifications represents <see cref="bool"/> values: true or false
/// </summary>
public static class BooleanSpecification
{
    /// <summary>
    /// <see cref="TrueSpecification"/>
    /// </summary>
    public static TrueSpecification True => TrueSpecification.Instance;
    
    /// <summary>
    /// <see cref="FalseSpecification"/>
    /// </summary>
    public static FalseSpecification False => FalseSpecification.Instance;
    
    /// <summary>
    /// Always returns true, singleton mode
    /// </summary>
    public sealed class TrueSpecification : ISpecification
    {
        public bool Result => true;
        
        private TrueSpecification() { }
        private static readonly Lazy<TrueSpecification> Inst = new(() => new TrueSpecification());
        public static TrueSpecification Instance => Inst.Value;
    }

    /// <summary>
    /// Always returns false, singleton mode
    /// </summary>
    public sealed class FalseSpecification : ISpecification
    {
        public bool Result => false;
        
        private FalseSpecification() { }
        private static readonly Lazy<FalseSpecification> Inst = new(() => new FalseSpecification());
        public static FalseSpecification Instance => Inst.Value;
    }
}