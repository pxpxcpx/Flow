namespace Flow.Shared.Enums;

[Flags]
public enum ProcessorStatus
{
    Ready     = 0b_0000_0001,
    Running   = 0b_0000_0010,
    Paused    = 0b_0000_0100,
    Failed    = 0b_0001_0000,
    Cancelled = 0b_0010_0000,
    Completed = 0b_0100_0000,
}
