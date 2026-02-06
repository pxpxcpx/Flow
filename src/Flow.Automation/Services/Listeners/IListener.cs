using Flow.Automation.Decisioning.Abstractions;
using Flow.Automation.Messaging;
using Flow.Shared.Enums;

namespace Flow.Automation.Services.Listeners;

public interface IListener
{
    string Name { get; }
    
    /// <summary>
    /// Conditions and its GUID.
    /// </summary>
    /// <remarks>The ID will be used as a shared key for the condition and its handler.</remarks>
    /// <seealso cref="MessageRouter{T}.Handlers"/>
    Dictionary<Guid, ICondition> Conditions { get; }
    
    /// <summary>
    /// Event stream for <see cref="IObserver{T}"/> objects to subscribe.
    /// </summary>
    IObservable<ListenerEventMessage> EventStream { get; }
    
    void Initialize();
    
    /// <summary>
    /// Listening asynchronously.
    /// </summary>
    Task StartAsync();
    
    /// <summary>
    /// Stop listening.
    /// </summary>
    Task StopAsync();
    
    ProcessorStatus Status { get; }
}