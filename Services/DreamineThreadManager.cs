using Dreamine.Logging.Interfaces;
using Dreamine.Threading.Interfaces;
using Dreamine.Threading.Models;

namespace Dreamine.Threading.Services;

/// <summary>
/// Provides the default Dreamine thread manager implementation.
/// </summary>
public sealed class DreamineThreadManager : IDreamineThreadManager
{
    private readonly object _syncRoot = new();
    private readonly List<IDreamineThread> _threads = new();
    private readonly IThreadCoreAllocator _coreAllocator;
    private readonly IThreadCyclePolicy _cyclePolicy;
    private readonly IDreamineThreadScheduler _scheduler;
    private readonly IThreadAffinityService? _affinityService;
    private readonly ITimerResolutionService? _timerResolutionService;
    private readonly IDreamineLogger? _logger;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DreamineThreadManager"/> class.
    /// </summary>
    public DreamineThreadManager(
        IThreadCoreAllocator coreAllocator,
        IThreadCyclePolicy cyclePolicy,
        IDreamineThreadScheduler scheduler,
        IThreadAffinityService? affinityService = null,
        ITimerResolutionService? timerResolutionService = null,
        IDreamineLogger? logger = null)
    {
        _coreAllocator = coreAllocator ?? throw new ArgumentNullException(nameof(coreAllocator));
        _cyclePolicy = cyclePolicy ?? throw new ArgumentNullException(nameof(cyclePolicy));
        _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
        _affinityService = affinityService;
        _timerResolutionService = timerResolutionService;
        _logger = logger;
    }

    /// <inheritdoc />
    public IDreamineThreadJob Register(
        DreamineThreadOptions options,
        Func<CancellationToken, ValueTask> action)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(action);

        lock (_syncRoot)
        {
            ThrowIfDisposed();
        }

        var normalized = options.Normalize();
        var assignment = _coreAllocator.Allocate(normalized);

        try
        {
            var jobOptions = new DreamineThreadJobOptions
            {
                Name = normalized.Name,
                IntervalMs = assignment.IsOverflowPolling
                    ? normalized.OverflowPollingIntervalMs
                    : normalized.IntervalMs,
                IsEnabled = true,
                IsOverflowPolling = assignment.IsOverflowPolling
            };

            var job = new DreamineThreadJob(jobOptions, action);

            // Worker decisions are taken under the lock; thread Start() is
            // performed afterwards to avoid running external callbacks
            // (logging, affinity, etc.) while holding the manager lock.
            IDreamineThread? workerToStart = null;

            lock (_syncRoot)
            {
                ThrowIfDisposed();

                if (assignment.IsOverflowPolling)
                {
                    workerToStart = RegisterOverflowJob(job);
                }
                else
                {
                    var worker = new DreamineThread(
                        normalized,
                        assignment,
                        _cyclePolicy,
                        _affinityService,
                        _timerResolutionService,
                        _logger);

                    worker.AddJob(job);
                    _threads.Add(worker);

                    if (normalized.AutoStart)
                    {
                        workerToStart = worker;
                    }
                }
            }

            workerToStart?.Start();

            _logger?.Info(
                $"Thread registered. Name={normalized.Name}, Core={assignment.CoreIndex}, Affinity={assignment.UseAffinity}");

            return job;
        }
        catch
        {
            _coreAllocator.Release(assignment);
            throw;
        }
    }

    /// <inheritdoc />
    public void StartAll()
    {
        foreach (var thread in GetThreads())
        {
            thread.Start();
        }
    }

    /// <inheritdoc />
    public void StopAll()
    {
        foreach (var thread in GetThreads())
        {
            thread.Stop();
        }
    }

    /// <inheritdoc />
    public bool Start(string threadName)
    {
        if (!TryGetThread(threadName, out var thread) || thread is null)
        {
            return false;
        }

        thread.Start();
        return true;
    }

    /// <inheritdoc />
    public bool Stop(string threadName)
    {
        if (!TryGetThread(threadName, out var thread) || thread is null)
        {
            return false;
        }

        thread.Stop();
        return true;
    }

    /// <inheritdoc />
    public bool Pause(string threadName)
    {
        if (!TryGetThread(threadName, out var thread) || thread is null)
        {
            return false;
        }

        thread.Pause();
        return true;
    }

    /// <inheritdoc />
    public bool Resume(string threadName)
    {
        if (!TryGetThread(threadName, out var thread) || thread is null)
        {
            return false;
        }

        thread.Resume();
        return true;
    }

    /// <inheritdoc />
    public void PauseAll()
    {
        foreach (var thread in GetThreads())
        {
            thread.Pause();
        }
    }

    /// <inheritdoc />
    public void ResumeAll()
    {
        foreach (var thread in GetThreads())
        {
            thread.Resume();
        }
    }

    /// <inheritdoc />
    public bool TryGetThread(string threadName, out IDreamineThread? thread)
    {
        thread = null;

        if (string.IsNullOrWhiteSpace(threadName))
        {
            return false;
        }

        lock (_syncRoot)
        {
            thread = _threads.FirstOrDefault(item =>
                string.Equals(item.Name, threadName, StringComparison.Ordinal));

            return thread is not null;
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<IDreamineThread> GetThreads()
    {
        lock (_syncRoot)
        {
            return _threads.ToArray();
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<DreamineThreadInfo> GetThreadInfos()
    {
        lock (_syncRoot)
        {
            return _threads
                .Select(thread => thread.GetInfo())
                .ToArray();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        StopAll();

        lock (_syncRoot)
        {
            foreach (var thread in _threads)
            {
                thread.Dispose();
                _coreAllocator.Release(thread.CoreAssignment);
            }

            _threads.Clear();
            _disposed = true;
        }
    }

    /// <summary>
    /// Routes an overflow polling job to an existing worker, or creates a
    /// fallback worker if none exists.
    /// </summary>
    /// <param name="job">The overflow polling job.</param>
    /// <returns>
    /// The worker to start outside the lock when a new fallback worker was
    /// created; <c>null</c> when an existing worker absorbed the job.
    /// </returns>
    /// <remarks>
    /// Must be called with <c>_syncRoot</c> held.
    /// </remarks>
    private IDreamineThread? RegisterOverflowJob(IDreamineThreadJob job)
    {
        var worker = _scheduler.SelectWorker(_threads);
        IDreamineThread? newWorker = null;

        if (worker is null)
        {
            var fallbackOptions = new DreamineThreadOptions
            {
                Name = "DreamineOverflowWorker",
                Priority = DreamineThreadPriority.Normal,
                IntervalMs = 10,
                CoreMode = DreamineThreadCoreMode.None,
                AutoStart = true
            };

            var fallbackAssignment = DreamineThreadCoreAssignment.None();

            newWorker = new DreamineThread(
                fallbackOptions,
                fallbackAssignment,
                _cyclePolicy,
                _affinityService,
                _timerResolutionService,
                _logger);

            _threads.Add(newWorker);
            worker = newWorker;
        }

        worker.AddJob(job);

        _logger?.Warning(
            $"Overflow polling job registered. Job={job.Name}, Worker={worker.Name}");

        return newWorker;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(DreamineThreadManager));
        }
    }
}
