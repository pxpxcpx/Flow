namespace Flow.Core.Abstractions.Enums;

[Flags]
internal enum FrameEvents
{
    None         = 0b_0000_0000,
    
    Initialize   = 0b_0000_0001,
    Pause        = 0b_0000_0010,
    Resume       = 0b_0000_0100,
    
    /// <summary>
    /// Terminate all activities in this frame.
    /// </summary>
    Terminate    = 0b_0000_1000,
    
    /// <summary>
    /// Move to next node, also includes sub function. 
    /// </summary>
    MoveNext     = 0b_0001_0000,
    
    /// <summary>
    /// Indicates the completion of a node.
    /// </summary>
    Continue     = 0b_0010_0000,
}