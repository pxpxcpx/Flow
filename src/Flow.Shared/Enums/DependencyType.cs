namespace Flow.Shared.Enums;

public enum DependencyType
{
    None    = 0b_0000_0000,
    Node    = 0b_0000_0001,
    Service = 0b_0000_0010,
    Package = 0b_0000_0100,
}