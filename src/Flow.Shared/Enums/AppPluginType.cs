namespace Flow.Shared.Enums;

public enum AppPluginType
{
    None      = 0b_0000_0000,
    Service   = 0b_0000_0010,
    Theme     = 0b_0000_0100,
    Component = 0b_0000_1000,
}