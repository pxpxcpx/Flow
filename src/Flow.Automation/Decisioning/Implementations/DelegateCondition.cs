namespace Flow.Automation.Decisioning.Implementations;

/// <summary>
/// Accept a <see cref="Predicate">delegate</see> as a condition of judgment.
/// </summary>
/// <param name="predicate">Delegate</param>
/// <param name="args">Nullable, parameters used to pass in the delegate.</param>
/// <typeparam name="T">Type of the object to be evaluated.</typeparam>
public class DelegateCondition<T>(Predicate<T> predicate, params object?[]? args) 
    : Condition<T>(args)
{
    public Predicate<T> Predicate { get; set; } = predicate;

    public override bool Result => Evaluate();

    public override bool Evaluate(T? obj = default)
    {
        return Predicate.Invoke(obj ?? throw new ArgumentNullException(nameof(obj)));
    }

    public static implicit operator DelegateCondition<T>(Predicate<T> predicate)
        => predicate.ToCondition();
}