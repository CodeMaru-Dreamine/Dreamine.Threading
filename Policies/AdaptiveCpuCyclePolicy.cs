using Dreamine.Threading.Interfaces;
using Dreamine.Threading.Models;

namespace Dreamine.Threading.Policies;

/// <summary>
/// Provides an adaptive cycle policy that increases delay based on process CPU usage.
/// </summary>
public sealed class AdaptiveCpuCyclePolicy : IThreadCyclePolicy
{
    private readonly ICpuUsageProvider _cpuUsageProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdaptiveCpuCyclePolicy"/> class.
    /// </summary>
    /// <param name="cpuUsageProvider">The CPU usage provider.</param>
    public AdaptiveCpuCyclePolicy(ICpuUsageProvider cpuUsageProvider)
    {
        _cpuUsageProvider = cpuUsageProvider ?? throw new ArgumentNullException(nameof(cpuUsageProvider));
    }

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

        if (options.IntervalMs > 0)
        {
            return options.IntervalMs;
        }

        if (options.IntervalMs < 0)
        {
            return 10;
        }

        if (!options.UseAdaptiveCpuDelay)
        {
            return 0;
        }

        var cpuUsage = _cpuUsageProvider.GetTotalCpuUsagePercent();

        if (cpuUsage >= 70)
        {
            return 5;
        }

        if (cpuUsage >= 50)
        {
            return 3;
        }

        if (cpuUsage >= 30)
        {
            return 1;
        }

        return 0;
    }
}