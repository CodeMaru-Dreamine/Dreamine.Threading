namespace Dreamine.Threading.Models;

/// <summary>
/// Represents options used to create a Dreamine worker thread.
/// </summary>
public sealed class DreamineThreadOptions
{
    /// <summary>
    /// Gets or sets the worker thread name.
    /// </summary>
    public string Name { get; set; } = "DreamineThread";

    /// <summary>
    /// Gets or sets the worker thread priority.
    /// </summary>
    public DreamineThreadPriority Priority { get; set; } = DreamineThreadPriority.Normal;

    /// <summary>
    /// Gets or sets the base cycle interval in milliseconds.
    /// </summary>
    public int IntervalMs { get; set; } = 10;

    /// <summary>
    /// Gets or sets the CPU core assignment mode.
    /// </summary>
    public DreamineThreadCoreMode CoreMode { get; set; } = DreamineThreadCoreMode.Auto;

    /// <summary>
    /// Gets or sets the manually assigned CPU core index.
    /// </summary>
    public int? CoreIndex { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of dedicated worker threads per CPU core in auto mode.
    /// </summary>
    public int AutoThreadsPerCore { get; set; } = 2;

    /// <summary>
    /// Gets or sets the polling interval used when a job is assigned to overflow polling.
    /// </summary>
    public int OverflowPollingIntervalMs { get; set; } = 100;

    /// <summary>
    /// Gets or sets a value indicating whether the worker should start immediately after creation.
    /// </summary>
    public bool AutoStart { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether high precision timer resolution is requested.
    /// </summary>
    public bool UseHighPrecisionTimer { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether adaptive CPU delay is enabled when the interval is zero.
    /// </summary>
    public bool UseAdaptiveCpuDelay { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the worker should yield when the interval is zero.
    /// </summary>
    public bool YieldWhenIntervalIsZero { get; set; } = true;

    /// <summary>
    /// Creates a normalized copy of the current options.
    /// </summary>
    /// <returns>The normalized options.</returns>
    public DreamineThreadOptions Normalize()
    {
        return new DreamineThreadOptions
        {
            Name = string.IsNullOrWhiteSpace(Name) ? "DreamineThread" : Name,
            Priority = Priority,
            IntervalMs = IntervalMs < 0 ? 10 : IntervalMs,
            CoreMode = CoreMode,
            CoreIndex = CoreIndex,
            AutoThreadsPerCore = AutoThreadsPerCore <= 0 ? 2 : AutoThreadsPerCore,
            OverflowPollingIntervalMs = OverflowPollingIntervalMs < 0 ? 100 : OverflowPollingIntervalMs,
            AutoStart = AutoStart,
            UseHighPrecisionTimer = UseHighPrecisionTimer,
            YieldWhenIntervalIsZero = YieldWhenIntervalIsZero,
            UseAdaptiveCpuDelay = UseAdaptiveCpuDelay
        };
    }
}