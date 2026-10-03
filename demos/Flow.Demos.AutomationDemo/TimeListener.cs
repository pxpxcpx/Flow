using Flow.Automation.Services.Listeners;

namespace Flow.Demos.AutomationDemo;

public class TimeListener : Listener<object>
{
    public override string Identifier { get; set; }
    
    public override string Description { get; set; }

    public override void Initialize()
    {
        throw new NotImplementedException();
    }

    public override Task StartAsync()
    {
        throw new NotImplementedException();
    }

    public override Task StopAsync()
    {
        throw new NotImplementedException();
    }
}