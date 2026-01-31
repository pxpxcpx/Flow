using Flow.Automation.Listeners;
using Timer = System.Timers.Timer;

namespace Flow.Automation.Messaging;

/// <summary>
/// PollingMessageBus based on a timer that checks a list of triggers at specified intervals
/// and publishes event messages when triggers are activated.
/// </summary>
public class PollingMessageBus : MessageBus<EventMessage>
{
    private readonly List<IPollingTrigger<EventArgs>> _triggers = new();

    private readonly Timer _timer;
    
    /// <summary>
    /// Timer's interval in milliseconds.
    /// </summary>
    public int Interval { get; init; }

    /// <summary>
    /// The count of ticks since the poller started.
    /// </summary>
    public int Ticks { get; private set; }
    
    /// <summary>
    /// Controls whether to report an empty event message on every tick even if no triggers were activated.
    /// </summary>
    public bool ReportEveryTime { get; init; } = false;

    public PollingMessageBus(int interval, List<IPollingTrigger<EventArgs>>? triggers = null)
    {
        if (triggers != null)
            _triggers = triggers;
        
        Interval = interval;
        _timer = new Timer(interval);
        _timer.AutoReset = false;
        _timer.Elapsed += (_, _) => Tick();
    }
    
    /// <summary>
    /// The handler for the timer's Elapsed event.
    /// It checks each trigger and publishes an event message if any are activated.
    /// </summary>
    private void Tick()
    {
        Ticks++;
        var hasMessage = false;
        
        foreach (var trigger in _triggers)
        {
            var args = trigger.CheckIfTriggered();
            
            if (args is null) continue;
            
            hasMessage = true;
            OnNext(new EventMessage(this, args));
        }
        
        if (!hasMessage && ReportEveryTime)
            OnNext(EventMessage.Empty);
    }
    
    /// <summary>
    /// Start the timer and begin polling triggers at the specified interval.
    /// </summary>
    public void Start()
        => _timer.Start();
    
    /// <summary>
    /// Stop the timer and halt polling triggers.
    /// </summary>
    public void Stop()
        => _timer.Stop();
    
    /// <inheritdoc cref="List{T}.Add"/>
    public void AddTrigger(IPollingTrigger<EventArgs> pollerTrigger)
        => _triggers.Add(pollerTrigger);
    
    /// <inheritdoc cref="List{T}.Remove"/>
    public void RemoveTrigger(IPollingTrigger<EventArgs> pollerTrigger)
        => _triggers.Remove(pollerTrigger);
    
    /// <inheritdoc cref="List{T}.Clear"/>
    public void ClearTriggers()
        => _triggers.Clear();

    public override void Dispose()
    {
        _timer.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}