using Dreamine.Threading.Models;

namespace Dreamine.Threading.Interfaces;

/// <summary>
/// Defines a policy that determines the delay between worker thread cycles.
/// </summary>
public interface IThreadCyclePolicy
{
    /// <summary>
    /// Gets the delay in milliseconds for the next worker cycle.
    /// </summary>
    /// <param name="options">The thread options.</param>
    /// <param name="assignment">The CPU core assignment.</param>
    /// <param name="context">The current thread cycle context.</param>
    /// <returns>The delay in milliseconds.</returns>
    int GetDelayMs(
        DreamineThreadOptions options,
        DreamineThreadCoreAssignment assignment,
        DreamineThreadCycleContext context);
}