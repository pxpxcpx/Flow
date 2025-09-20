using Flow.Automation.Decisioning.Abstractions;

namespace Flow.Automation.Messaging.Trigger;

public interface ITrigger<in T> : ISpecification, IObserver<T>;