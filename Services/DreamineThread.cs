using Dreamine.Logging.Interfaces;
using Dreamine.Threading.Interfaces;
using Dreamine.Threading.Models;

namespace Dreamine.Threading.Services;

/// <summary>
/// Provides the default Dreamine worker thread implementation.
/// </summary>
public sealed class DreamineThread : IDreamineThread
{
    private readonly object _syncRoot = new();
    private readonly List<IDreamineThreadJob> _jobs = new();
    private readonly IThreadCyclePolicy _cyclePolicy;
    private readonly IThreadAffinityService? _affinityService;
    private readonly ITimerResolutionService? _timerResolutionService;
    private readonly IDreamineLogger? _logger;
    private readonly ManualResetEventSlim _pauseEvent = new(true);

    private Thread? _thread;
    private CancellationTokenSource? _cancellationTokenSource;
    private long _cycleCount;
    private DateTimeOffset? _startedAt;
    private DateTimeOffset? _stoppedAt;
    private string? _lastErrorMessage;
    private bool _disposed;

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public DreamineThreadOptions Options { get; }

    /// <inheritdoc />
    public DreamineThreadCoreAssignment CoreAssignment { get; }

    /// <inheritdoc />
    public DreamineThreadStatus Status { get; private set; } = DreamineThreadStatus.Created;

    /// <inheritdoc />
    public int JobCount
    {
        get
        {
            lock (_syncRoot)
            {
                return _jobs.Count;
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DreamineThread"/> class.
    /// </summary>
    /// <param name="options">The worker thread options.</param>
    /// <param name="coreAssignment">The CPU core assignment.</param>
    /// <param name="cyclePolicy">The cycle policy.</param>
    /// <param name="affinityService">The optional CPU affinity service.</param>
    /// <param name="timerResolutionService">The optional timer resolution service.</param>
    /// <param name="logger">The optional logger.</param>
    public DreamineThread(
        DreamineThreadOptions options,
        DreamineThreadCoreAssignment coreAssignment,
        IThreadCyclePolicy cyclePolicy,
        IThreadAffinityService? affinityService = null,
        ITimerResolutionService? timerResolutionService = null,
        IDreamineLogger? logger = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(coreAssignment);
        ArgumentNullException.ThrowIfNull(cyclePolicy);

        Options = options.Normalize();
        Name = Options.Name;
        CoreAssignment = coreAssignment;
        _cyclePolicy = cyclePolicy;
        _affinityService = affinityService;
        _timerResolutionService = timerResolutionService;
        _logger = logger;
    }

    /// <inheritdoc />
    public void AddJob(IDreamineThreadJob job)
    {
        ArgumentNullException.ThrowIfNull(job);

        lock (_syncRoot)
        {
            ThrowIfDisposed();
            _jobs.Add(job);
        }
    }

    /// <inheritdoc />
    public void Start()
    {
        Thread? threadToStart = null;

        lock (_syncRoot)
        {
            ThrowIfDisposed();

            if (Status == DreamineThreadStatus.Running)
            {
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _pauseEvent.Set();

            _thread = new Thread(() => Run(_cancellationTokenSource.Token))
            {
                IsBackground = true,
                Name = Name,
                Priority = MapPriority(Options.Priority)
            };

            Status = DreamineThreadStatus.Running;
            _startedAt = DateTimeOffset.Now;
            _stoppedAt = null;
            _lastErrorMessage = null;

            threadToStart = _thread;
        }

        // Start the thread outside the lock so that any logging or scheduling
        // callbacks triggered by the OS during startup cannot deadlock against
        // the lock held by callers like the manager.
        threadToStart?.Start();

        _logger?.Info($"Thread started. Name={Name}");
    }

    /// <inheritdoc />
    public void Pause()
    {
        bool transitioned;

        lock (_syncRoot)
        {
            if (Status != DreamineThreadStatus.Running)
            {
                transitioned = false;
            }
            else
            {
                _pauseEvent.Reset();
                Status = DreamineThreadStatus.Paused;
                transitioned = true;
            }
        }

        if (transitioned)
        {
            _logger?.Info($"Thread paused. Name={Name}");
        }
    }

    /// <inheritdoc />
    public void Resume()
    {
        bool transitioned;

        lock (_syncRoot)
        {
            if (Status != DreamineThreadStatus.Paused)
            {
                transitioned = false;
            }
            else
            {
                _pauseEvent.Set();
                Status = DreamineThreadStatus.Running;
                transitioned = true;
            }
        }

        if (transitioned)
        {
            _logger?.Info($"Thread resumed. Name={Name}");
        }
    }

    /// <inheritdoc />
    public void Stop()
    {
        Thread? threadToJoin;

        lock (_syncRoot)
        {
            if (Status is DreamineThreadStatus.Stopped or DreamineThreadStatus.Disposed)
            {
                return;
            }

            Status = DreamineThreadStatus.Stopping;
            _pauseEvent.Set();

            _cancellationTokenSource?.Cancel();
            threadToJoin = _thread;
        }

        if (threadToJoin is not null && threadToJoin.IsAlive)
        {
            threadToJoin.Join(TimeSpan.FromSeconds(2));
        }

        lock (_syncRoot)
        {
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            _thread = null;

            if (Status != DreamineThreadStatus.Faulted)
            {
                Status = DreamineThreadStatus.Stopped;
            }

            _stoppedAt = DateTimeOffset.Now;
        }

        _logger?.Info($"Thread stopped. Name={Name}");
    }

    /// <inheritdoc />
    public DreamineThreadInfo GetInfo()
    {
        return new DreamineThreadInfo(
            Name,
            Status,
            Options.Priority,
            Options.IntervalMs,
            CoreAssignment.CoreIndex,
            CoreAssignment.UseAffinity,
            JobCount,
            Interlocked.Read(ref _cycleCount),
            _startedAt,
            _stoppedAt,
            _lastErrorMessage);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Stop();

        _pauseEvent.Dispose();
        _disposed = true;
        Status = DreamineThreadStatus.Disposed;
    }

    private void Run(CancellationToken cancellationToken)
    {
        try
        {
            if (Options.UseHighPrecisionTimer)
            {
                _timerResolutionService?.Begin();
            }

            if (CoreAssignment.UseAffinity && CoreAssignment.CoreIndex is not null)
            {
                _affinityService?.ApplyToCurrentThread(CoreAssignment.CoreIndex.Value);
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                _pauseEvent.Wait(cancellationToken);

                ExecuteDueJobs(cancellationToken);

                Interlocked.Increment(ref _cycleCount);

                var context = new DreamineThreadCycleContext(
                    Name,
                    Interlocked.Read(ref _cycleCount),
                    JobCount,
                    CoreAssignment.CoreIndex,
                    CoreAssignment.IsOverflowPolling,
                    DateTimeOffset.Now);

                var delayMs = _cyclePolicy.GetDelayMs(
                    Options,
                    CoreAssignment,
                    context);

                if (delayMs > 0)
                {
                    cancellationToken.WaitHandle.WaitOne(delayMs);
                }
                else if (Options.YieldWhenIntervalIsZero)
                {
                    Thread.Yield();
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Normal stop path.
        }
        catch (Exception ex)
        {
            _lastErrorMessage = ex.Message;
            Status = DreamineThreadStatus.Faulted;
            _logger?.Error(ex, $"Thread faulted. Name={Name}");
        }
        finally
        {
            if (Options.UseHighPrecisionTimer)
            {
                _timerResolutionService?.End();
            }

            if (CoreAssignment.UseAffinity)
            {
                _affinityService?.ClearCurrentThreadAffinity();
            }
        }
    }

    private void ExecuteDueJobs(CancellationToken cancellationToken)
    {
        IDreamineThreadJob[] jobs;

        lock (_syncRoot)
        {
            jobs = _jobs.ToArray();
        }

        var now = DateTimeOffset.Now;

        foreach (var job in jobs)
        {
            if (!job.ShouldRun(now))
            {
                continue;
            }

            try
            {
                job.ExecuteAsync(cancellationToken).AsTask().GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _lastErrorMessage = ex.Message;
                _logger?.Error(ex, $"Thread job failed. Thread={Name}, Job={job.Name}");
            }
        }
    }

    private static ThreadPriority MapPriority(DreamineThreadPriority priority)
    {
        return priority switch
        {
            DreamineThreadPriority.High => ThreadPriority.AboveNormal,
            DreamineThreadPriority.Low => ThreadPriority.BelowNormal,
            _ => ThreadPriority.Normal
        };
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(DreamineThread));
        }
    }
}
