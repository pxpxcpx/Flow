namespace Flow.Shared.Enums;

public enum PluginType
{
    None      = 0b_0000_0000,
    Node      = 0b_0000_0001,
    Service   = 0b_0000_0010,
    AppPlugin = 0b_0000_0100,
    Theme     = 0b_0000_1000,
    Component = 0b_0001_0000,
}