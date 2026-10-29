using Flow.Core.Models.Controlling;

namespace Flow.Core.Abstractions.Enums;

/// <summary>
/// Represents the status of <see cref="Frame"/>.
/// </summary>
[Flags]
internal enum FrameStates
{
    None              = 0b_0000_0000,
    
    /// <summary>
    /// Frame is ready to be executed.
    /// </summary>
    Ready             = 0b_0000_0010,
    
    /// <summary>
    /// Pause or has suspended due to unexpected conditions.
    /// </summary>
    Suspended         = 0b_0000_0100,
    
    /// <summary>
    /// End of frame's lifecycle.
    /// </summary>
    Terminated        = 0b_0000_1000,
}