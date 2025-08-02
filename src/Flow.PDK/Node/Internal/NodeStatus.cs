namespace Flow.PDK.Node.Internal;

public enum NodeStatus
{
    Ready = 0,
    Running = 1,
    Completed = 2,
    Failed = 4,
    Skipped = 8,
    Canceled = 16
}
