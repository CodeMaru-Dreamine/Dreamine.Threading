namespace Dreamine.Threading.Models;

/// <summary>
/// Defines how a Dreamine thread is assigned to CPU cores.
/// </summary>
public enum DreamineThreadCoreMode
{
    /// <summary>
    /// No CPU affinity is applied.
    /// </summary>
    None = 0,

    /// <summary>
    /// CPU core assignment is selected automatically.
    /// </summary>
    Auto = 1,

    /// <summary>
    /// CPU core assignment is selected manually.
    /// </summary>
    Manual = 2
}