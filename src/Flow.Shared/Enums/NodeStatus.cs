namespace Flow.Shared.Enums;

[Flags]
public enum NodeStatus
{
    Ready      = 0b_0000_0000,
    Running    = 0b_0000_0001,
    Waiting    = 0b_0000_0010,
    Completed  = 0b_0000_0100,
    Failed     = 0b_0000_1000,
    Skipped    = 0b_0001_0000,
    Cancelled  = 0b_0010_0000,
}