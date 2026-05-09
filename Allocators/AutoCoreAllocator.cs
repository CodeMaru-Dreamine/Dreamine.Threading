using Dreamine.Threading.Interfaces;
using Dreamine.Threading.Models;

namespace Dreamine.Threading.Allocators;

/// <summary>
/// Allocates CPU cores for Dreamine worker threads using an automatic per-core capacity policy.
/// </summary>
public sealed class AutoCoreAllocator : IThreadCoreAllocator
{
    private readonly object _syncRoot = new();
    private readonly int _logicalCoreCount;
    private readonly int[] _assignedCounts;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoCoreAllocator"/> class.
    /// </summary>
    public AutoCoreAllocator()
        : this(Environment.ProcessorCount)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoCoreAllocator"/> class.
    /// </summary>
    /// <param name="logicalCoreCount">The logical CPU core count.</param>
    public AutoCoreAllocator(int logicalCoreCount)
    {
        _logicalCoreCount = logicalCoreCount <= 0 ? 1 : logicalCoreCount;
        _assignedCounts = new int[_logicalCoreCount];
    }

    /// <inheritdoc />
    public DreamineThreadCoreAssignment Allocate(DreamineThreadOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var normalized = options.Normalize();

        lock (_syncRoot)
        {
            return normalized.CoreMode switch
            {
                DreamineThreadCoreMode.None => DreamineThreadCoreAssignment.None(),
                DreamineThreadCoreMode.Manual => AllocateManual(normalized),
                DreamineThreadCoreMode.Auto => AllocateAuto(normalized),
                _ => DreamineThreadCoreAssignment.None()
            };
        }
    }

    /// <inheritdoc />
    public void Release(DreamineThreadCoreAssignment assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);

        if (assignment.CoreIndex is null || assignment.IsOverflowPolling)
        {
            return;
        }

        lock (_syncRoot)
        {
            var coreIndex = assignment.CoreIndex.Value;

            if (coreIndex < 0 || coreIndex >= _assignedCounts.Length)
            {
                return;
            }

            if (_assignedCounts[coreIndex] > 0)
            {
                _assignedCounts[coreIndex]--;
            }
        }
    }

    private DreamineThreadCoreAssignment AllocateManual(DreamineThreadOptions options)
    {
        if (options.CoreIndex is null)
        {
            return DreamineThreadCoreAssignment.None();
        }

        var coreIndex = options.CoreIndex.Value;

        if (coreIndex < 0 || coreIndex >= _logicalCoreCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                $"CPU core index {coreIndex} is out of range. Logical core count is {_logicalCoreCount}.");
        }

        _assignedCounts[coreIndex]++;
        return DreamineThreadCoreAssignment.Dedicated(coreIndex, true);
    }

    private DreamineThreadCoreAssignment AllocateAuto(DreamineThreadOptions options)
    {
        var maxPerCore = options.AutoThreadsPerCore <= 0 ? 2 : options.AutoThreadsPerCore;

        var selectedCore = 0;
        var selectedCount = _assignedCounts[0];

        for (var i = 1; i < _assignedCounts.Length; i++)
        {
            if (_assignedCounts[i] < selectedCount)
            {
                selectedCore = i;
                selectedCount = _assignedCounts[i];
            }
        }

        if (selectedCount >= maxPerCore)
        {
            return DreamineThreadCoreAssignment.Overflow();
        }

        _assignedCounts[selectedCore]++;
        return DreamineThreadCoreAssignment.Dedicated(selectedCore, true);
    }
}