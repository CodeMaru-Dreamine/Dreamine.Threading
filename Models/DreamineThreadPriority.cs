namespace Dreamine.Threading.Models;

/// <summary>
/// Defines the priority level of a Dreamine thread.
/// </summary>
public enum DreamineThreadPriority
{
    /// <summary>
    /// Low priority for background or monitoring work.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Normal priority for standard work.
    /// </summary>
    Normal = 1,

    /// <summary>
    /// High priority for timing-sensitive work.
    /// </summary>
    High = 2
}