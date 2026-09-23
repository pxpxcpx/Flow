using Flow.Automation.Messaging.Abstractions;
using Flow.Shared.Abstractions;
using Flow.Shared.Enums;

namespace Flow.Automation.Services.Abstractions;

public interface IListener<out TMessage> : IObservable<TMessage>, IDescribable, ISubscriptionManaged
{
    /// <summary>
    /// Initial the listener.
    /// </summary>
    void Initialize();
    
    /// <summary>
    /// Listening asynchronously.
    /// </summary>
    Task StartAsync();
    
    /// <summary>
    /// Stop listening.
    /// </summary>
    Task StopAsync();
    
    /// <summary>
    /// Status of the listener.
    /// </summary>
    ProcessorStatus Status { get; }
}