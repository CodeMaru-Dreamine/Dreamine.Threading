namespace Dreamine.Threading.Models;

/// <summary>
/// Represents options used to create a Dreamine thread job.
/// </summary>
public sealed class DreamineThreadJobOptions
{
    /// <summary>
    /// Gets or sets the job name.
    /// </summary>
    public string Name { get; set; } = "DreamineThreadJob";

    /// <summary>
    /// Gets or sets the job execution interval in milliseconds.
    /// </summary>
    public int IntervalMs { get; set; } = 10;

    /// <summary>
    /// Gets or sets a value indicating whether the job is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the job is an overflow polling job.
    /// </summary>
    public bool IsOverflowPolling { get; set; }

    /// <summary>
    /// Creates a normalized copy of the current options.
    /// </summary>
    /// <returns>The normalized options.</returns>
    public DreamineThreadJobOptions Normalize()
    {
        return new DreamineThreadJobOptions
        {
            Name = string.IsNullOrWhiteSpace(Name) ? "DreamineThreadJob" : Name,
            IntervalMs = IntervalMs < 0 ? 10 : IntervalMs,
            IsEnabled = IsEnabled,
            IsOverflowPolling = IsOverflowPolling
        };
    }
}