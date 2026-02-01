using Flow.Automation.Decisioning.Abstractions;

namespace Flow.Automation.Decisioning.Implementations;

/// <summary>
/// Base class of <see cref="IParamCondition{T}"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Condition<T> : IParamCondition<T>
{
    public object?[]? Conditions { get; set; }
    
    public virtual bool Result => Evaluate();
    
    /// <summary>
    /// Base class of <see cref="IParamCondition{T}"/>.
    /// </summary>
    /// <param name="args"></param>
    /// <typeparam name="T"></typeparam>
    public Condition(params object?[]? args)
    {
        Conditions = args;
    }

    protected Condition()
    {
    }
    
    public abstract bool Evaluate(T? obj = default);

    public virtual void SetConditions(object?[]? conditions) 
        => Conditions = conditions;
}
