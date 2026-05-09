namespace Dreamine.Threading.Models;

/// <summary>
/// Represents runtime context information for a Dreamine thread cycle.
/// </summary>
public sealed class DreamineThreadCycleContext
{
    /// <summary>
    /// Gets the worker thread name.
    /// </summary>
    public string ThreadName { get; }

    /// <summary>
    /// Gets the current cycle count.
    /// </summary>
    public long CycleCount { get; }

    /// <summary>
    /// Gets the number of jobs assigned to the worker.
    /// </summary>
    public int JobCount { get; }

    /// <summary>
    /// Gets the assigned CPU core index.
    /// </summary>
    public int? CoreIndex { get; }

    /// <summary>
    /// Gets a value indicating whether this cycle is for overflow polling.
    /// </summary>
    public bool IsOverflowPolling { get; }

    /// <summary>
    /// Gets the current cycle timestamp.
    /// </summary>
    public DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DreamineThreadCycleContext"/> class.
    /// </summary>
    /// <param name="threadName">The worker thread name.</param>
    /// <param name="cycleCount">The current cycle count.</param>
    /// <param name="jobCount">The number of assigned jobs.</param>
    /// <param name="coreIndex">The assigned CPU core index.</param>
    /// <param name="isOverflowPolling">Whether this cycle is for overflow polling.</param>
    /// <param name="timestamp">The current timestamp.</param>
    public DreamineThreadCycleContext(
        string threadName,
        long cycleCount,
        int jobCount,
        int? coreIndex,
        bool isOverflowPolling,
        DateTimeOffset timestamp)
    {
        ThreadName = string.IsNullOrWhiteSpace(threadName)
            ? "DreamineThread"
            : threadName;

        CycleCount = cycleCount;
        JobCount = jobCount;
        CoreIndex = coreIndex;
        IsOverflowPolling = isOverflowPolling;
        Timestamp = timestamp;
    }
}