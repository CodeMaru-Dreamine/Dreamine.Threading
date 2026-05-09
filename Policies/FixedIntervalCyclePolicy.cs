using Dreamine.Threading.Interfaces;
using Dreamine.Threading.Models;

namespace Dreamine.Threading.Policies;

/// <summary>
/// Provides a fixed interval cycle policy for Dreamine worker threads.
/// </summary>
public sealed class FixedIntervalCyclePolicy : IThreadCyclePolicy
{
    /// <inheritdoc />
    public int GetDelayMs(
        DreamineThreadOptions options,
        DreamineThreadCoreAssignment assignment,
        DreamineThreadCycleContext context)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(assignment);
        ArgumentNullException.ThrowIfNull(context);

        if (assignment.IsOverflowPolling)
        {
            return options.OverflowPollingIntervalMs < 0
                ? 100
                : options.OverflowPollingIntervalMs;
        }

        return options.IntervalMs < 0
            ? 10
            : options.IntervalMs;
    }
}