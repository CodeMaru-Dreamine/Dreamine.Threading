using Dreamine.Threading.Interfaces;
using Dreamine.Threading.Models;

namespace Dreamine.Threading.Policies;

/// <summary>
/// Provides a polling interval policy for overflow jobs.
/// </summary>
public sealed class OverflowPollingPolicy : IThreadCyclePolicy
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

        return options.OverflowPollingIntervalMs < 0
            ? 100
            : options.OverflowPollingIntervalMs;
    }
}