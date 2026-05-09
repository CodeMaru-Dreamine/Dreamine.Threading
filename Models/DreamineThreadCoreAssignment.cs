namespace Dreamine.Threading.Models;

/// <summary>
/// Represents the CPU core assignment result for a Dreamine worker thread.
/// </summary>
public sealed class DreamineThreadCoreAssignment
{
    /// <summary>
    /// Gets the assigned CPU core index.
    /// </summary>
    public int? CoreIndex { get; }

    /// <summary>
    /// Gets a value indicating whether CPU affinity should be applied.
    /// </summary>
    public bool UseAffinity { get; }

    /// <summary>
    /// Gets a value indicating whether this assignment represents an overflow polling job.
    /// </summary>
    public bool IsOverflowPolling { get; }

    /// <summary>
    /// Gets a value indicating whether this assignment has a dedicated worker thread.
    /// </summary>
    public bool IsDedicatedWorker => !IsOverflowPolling;

    /// <summary>
    /// Initializes a new instance of the <see cref="DreamineThreadCoreAssignment"/> class.
    /// </summary>
    /// <param name="coreIndex">The assigned CPU core index.</param>
    /// <param name="useAffinity">Whether CPU affinity should be applied.</param>
    /// <param name="isOverflowPolling">Whether this assignment is for overflow polling.</param>
    public DreamineThreadCoreAssignment(int? coreIndex, bool useAffinity, bool isOverflowPolling)
    {
        CoreIndex = coreIndex;
        UseAffinity = useAffinity;
        IsOverflowPolling = isOverflowPolling;
    }

    /// <summary>
    /// Creates an assignment without CPU affinity.
    /// </summary>
    /// <returns>The created assignment.</returns>
    public static DreamineThreadCoreAssignment None()
    {
        return new DreamineThreadCoreAssignment(null, false, false);
    }

    /// <summary>
    /// Creates a dedicated CPU core assignment.
    /// </summary>
    /// <param name="coreIndex">The assigned CPU core index.</param>
    /// <param name="useAffinity">Whether CPU affinity should be applied.</param>
    /// <returns>The created assignment.</returns>
    public static DreamineThreadCoreAssignment Dedicated(int coreIndex, bool useAffinity)
    {
        return new DreamineThreadCoreAssignment(coreIndex, useAffinity, false);
    }

    /// <summary>
    /// Creates an overflow polling assignment.
    /// </summary>
    /// <returns>The created assignment.</returns>
    public static DreamineThreadCoreAssignment Overflow()
    {
        return new DreamineThreadCoreAssignment(null, false, true);
    }
}