namespace Dreamine.Threading.Models;

/// <summary>
/// Represents a snapshot of a Dreamine worker thread state.
/// </summary>
public sealed class DreamineThreadInfo
{
    /// <summary>
    /// Gets the worker thread name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the worker thread status.
    /// </summary>
    public DreamineThreadStatus Status { get; }

    /// <summary>
    /// Gets the worker thread priority.
    /// </summary>
    public DreamineThreadPriority Priority { get; }

    /// <summary>
    /// Gets the worker thread interval in milliseconds.
    /// </summary>
    public int IntervalMs { get; }

    /// <summary>
    /// Gets the assigned CPU core index.
    /// </summary>
    public int? CoreIndex { get; }

    /// <summary>
    /// Gets a value indicating whether CPU affinity is enabled.
    /// </summary>
    public bool UseAffinity { get; }

    /// <summary>
    /// Gets the number of jobs assigned to the worker.
    /// </summary>
    public int JobCount { get; }

    /// <summary>
    /// Gets the number of completed cycles.
    /// </summary>
    public long CycleCount { get; }

    /// <summary>
    /// Gets the last started time.
    /// </summary>
    public DateTimeOffset? StartedAt { get; }

    /// <summary>
    /// Gets the last stopped time.
    /// </summary>
    public DateTimeOffset? StoppedAt { get; }

    /// <summary>
    /// Gets the last exception message.
    /// </summary>
    public string? LastErrorMessage { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DreamineThreadInfo"/> class.
    /// </summary>
    public DreamineThreadInfo(
        string name,
        DreamineThreadStatus status,
        DreamineThreadPriority priority,
        int intervalMs,
        int? coreIndex,
        bool useAffinity,
        int jobCount,
        long cycleCount,
        DateTimeOffset? startedAt,
        DateTimeOffset? stoppedAt,
        string? lastErrorMessage)
    {
        Name = name;
        Status = status;
        Priority = priority;
        IntervalMs = intervalMs;
        CoreIndex = coreIndex;
        UseAffinity = useAffinity;
        JobCount = jobCount;
        CycleCount = cycleCount;
        StartedAt = startedAt;
        StoppedAt = stoppedAt;
        LastErrorMessage = lastErrorMessage;
    }
}