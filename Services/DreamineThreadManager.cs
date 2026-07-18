using Dreamine.Logging.Interfaces;
using Dreamine.Threading.Interfaces;
using Dreamine.Threading.Models;

namespace Dreamine.Threading.Services;

/// <summary>
/// \if KO
/// <para>기본 Dreamine 작업자 스레드 관리자 구현을 제공합니다.</para>
/// \endif
/// \if EN
/// <para>Provides the default Dreamine worker-thread manager implementation.</para>
/// \endif
/// </summary>
public sealed class DreamineThreadManager : IDreamineThreadManager
{
    /// <summary>
    /// \if KO
    /// <para>sync Root 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the sync root value.</para>
    /// \endif
    /// </summary>
    private readonly object _syncRoot = new();
    /// <summary>
    /// \if KO
    /// <para>threads 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the threads value.</para>
    /// \endif
    /// </summary>
    private readonly List<IDreamineThread> _threads = new();
    /// <summary>
    /// \if KO
    /// <para>core Allocator 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the core allocator value.</para>
    /// \endif
    /// </summary>
    private readonly IThreadCoreAllocator _coreAllocator;
    /// <summary>
    /// \if KO
    /// <para>cycle Policy 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the cycle policy value.</para>
    /// \endif
    /// </summary>
    private readonly IThreadCyclePolicy _cyclePolicy;
    /// <summary>
    /// \if KO
    /// <para>scheduler 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the scheduler value.</para>
    /// \endif
    /// </summary>
    private readonly IDreamineThreadScheduler _scheduler;
    /// <summary>
    /// \if KO
    /// <para>affinity Service 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the affinity service value.</para>
    /// \endif
    /// </summary>
    private readonly IThreadAffinityService? _affinityService;
    /// <summary>
    /// \if KO
    /// <para>timer Resolution Service 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the timer resolution service value.</para>
    /// \endif
    /// </summary>
    private readonly ITimerResolutionService? _timerResolutionService;
    /// <summary>
    /// \if KO
    /// <para>logger 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the logger value.</para>
    /// \endif
    /// </summary>
    private readonly IDreamineLogger? _logger;
    /// <summary>
    /// \if KO
    /// <para>disposed 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the disposed value.</para>
    /// \endif
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// \if KO
    /// <para>코어 할당, 주기 정책 및 스케줄링 서비스를 사용해 <see cref="T:Dreamine.Threading.Services.DreamineThreadManager" /> 클래스의 새 인스턴스를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes a new instance of <see cref="T:Dreamine.Threading.Services.DreamineThreadManager" /> using core-allocation, cycle-policy, and scheduling services.</para>
    /// \endif
    /// </summary>
    /// <param name="coreAllocator">
    /// \if KO
    /// <para>스레드의 CPU 코어를 할당하고 해제할 서비스입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The service that allocates and releases CPU cores for threads.</para>
    /// \endif
    /// </param>
    /// <param name="cyclePolicy">
    /// \if KO
    /// <para>작업자 실행 주기 지연 정책입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The worker execution-cycle delay policy.</para>
    /// \endif
    /// </param>
    /// <param name="scheduler">
    /// \if KO
    /// <para>오버플로 작업을 기존 작업자에 배치할 스케줄러입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The scheduler that routes overflow jobs to existing workers.</para>
    /// \endif
    /// </param>
    /// <param name="affinityService">
    /// \if KO
    /// <para>선택적 CPU 선호도 서비스입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The optional CPU-affinity service.</para>
    /// \endif
    /// </param>
    /// <param name="timerResolutionService">
    /// \if KO
    /// <para>선택적 타이머 해상도 서비스입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The optional timer-resolution service.</para>
    /// \endif
    /// </param>
    /// <param name="logger">
    /// \if KO
    /// <para>선택적 진단 로거입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The optional diagnostic logger.</para>
    /// \endif
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// \if KO
    /// <para><paramref name="coreAllocator"/>, <paramref name="cyclePolicy"/> 또는 <paramref name="scheduler"/>가 <see langword="null"/>일 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when <paramref name="coreAllocator"/>, <paramref name="cyclePolicy"/>, or <paramref name="scheduler"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
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

    /// <summary>
    /// \if KO
    /// <para>옵션에 따라 전용 작업자 또는 기존 작업자의 오버플로 작업을 등록합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Registers either a dedicated worker or an overflow job on an existing worker according to the options.</para>
    /// \endif
    /// </summary>
    /// <param name="options">
    /// \if KO
    /// <para>정규화할 스레드·코어·주기 옵션입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The thread, core, and cycle options to normalize.</para>
    /// \endif
    /// </param>
    /// <param name="action">
    /// \if KO
    /// <para>등록할 비동기 작업 대리자입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The asynchronous job delegate to register.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>생성되어 작업자에 할당된 스레드 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The created thread job assigned to a worker.</para>
    /// \endif
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// \if KO
    /// <para><paramref name="options"/> 또는 <paramref name="action"/>이 <see langword="null"/>일 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when <paramref name="options"/> or <paramref name="action"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// \if KO
    /// <para>관리자가 이미 정리된 경우 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the manager has already been disposed.</para>
    /// \endif
    /// </exception>
    /// <remarks>
    /// \if KO
    /// <para>등록 실패 시 이미 얻은 코어 할당을 할당기에 반환합니다. 작업자 시작은 관리자 잠금 밖에서 수행됩니다.</para>
    /// \endif
    /// \if EN
    /// <para>A core assignment already obtained is returned to the allocator on registration failure. Worker startup occurs outside the manager lock.</para>
    /// \endif
    /// </remarks>
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

    /// <summary>
    /// \if KO
    /// <para>등록된 모든 작업자 스레드의 스냅샷을 순회해 시작합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Starts every registered worker thread from a snapshot of the collection.</para>
    /// \endif
    /// </summary>
    public void StartAll()
    {
        foreach (var thread in GetThreads())
        {
            thread.Start();
        }
    }

    /// <summary>
    /// \if KO
    /// <para>등록된 모든 작업자 스레드를 순서대로 동기 중지합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Synchronously stops every registered worker thread in order.</para>
    /// \endif
    /// </summary>
    /// <remarks>
    /// \if KO
    /// <para>각 작업자의 join 대기 동안 호출 스레드가 차단될 수 있습니다.</para>
    /// \endif
    /// \if EN
    /// <para>The caller can be blocked during each worker's join wait.</para>
    /// \endif
    /// </remarks>
    public void StopAll()
    {
        foreach (var thread in GetThreads())
        {
            thread.Stop();
        }
    }

    /// <summary>
    /// \if KO
    /// <para>등록된 모든 작업자 스레드를 순서대로 비동기 중지합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Asynchronously stops every registered worker thread in order.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>모든 작업자의 순차 중지 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The sequential stop operation for all workers.</para>
    /// \endif
    /// </returns>
    public async ValueTask StopAllAsync()
    {
        foreach (var thread in GetThreads())
        {
            await thread.StopAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// \if KO
    /// <para>이름이 일치하는 작업자 스레드를 시작합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Starts the worker thread whose name matches.</para>
    /// \endif
    /// </summary>
    /// <param name="threadName">
    /// \if KO
    /// <para>시작할 작업자 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The worker name to start.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>작업자를 찾아 시작했으면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when the worker was found and started.</para>
    /// \endif
    /// </returns>
    public bool Start(string threadName)
    {
        if (!TryGetThread(threadName, out var thread) || thread is null)
        {
            return false;
        }

        thread.Start();
        return true;
    }

    /// <summary>
    /// \if KO
    /// <para>이름이 일치하는 작업자 스레드를 동기 중지합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Synchronously stops the worker thread whose name matches.</para>
    /// \endif
    /// </summary>
    /// <param name="threadName">
    /// \if KO
    /// <para>중지할 작업자 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The worker name to stop.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>작업자를 찾아 중지했으면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when the worker was found and stopped.</para>
    /// \endif
    /// </returns>
    /// <remarks>
    /// \if KO
    /// <para>작업자의 join 대기 동안 호출 스레드가 차단될 수 있습니다.</para>
    /// \endif
    /// \if EN
    /// <para>The caller can be blocked during the worker's join wait.</para>
    /// \endif
    /// </remarks>
    public bool Stop(string threadName)
    {
        if (!TryGetThread(threadName, out var thread) || thread is null)
        {
            return false;
        }

        thread.Stop();
        return true;
    }

    /// <summary>
    /// \if KO
    /// <para>이름이 일치하는 작업자 스레드를 비동기 중지합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Asynchronously stops the worker thread whose name matches.</para>
    /// \endif
    /// </summary>
    /// <param name="threadName">
    /// \if KO
    /// <para>중지할 작업자 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The worker name to stop.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>작업자를 찾아 중지했는지를 나타내는 비동기 결과입니다.</para>
    /// \endif
    /// \if EN
    /// <para>An asynchronous result indicating whether the worker was found and stopped.</para>
    /// \endif
    /// </returns>
    public async ValueTask<bool> StopAsync(string threadName)
    {
        if (!TryGetThread(threadName, out var thread) || thread is null)
        {
            return false;
        }

        await thread.StopAsync().ConfigureAwait(false);
        return true;
    }

    /// <summary>
    /// \if KO
    /// <para>이름이 일치하는 작업자 스레드를 일시 정지합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Pauses the worker thread whose name matches.</para>
    /// \endif
    /// </summary>
    /// <param name="threadName">
    /// \if KO
    /// <para>일시 정지할 작업자 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The worker name to pause.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>작업자를 찾아 일시 정지했으면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when the worker was found and paused.</para>
    /// \endif
    /// </returns>
    public bool Pause(string threadName)
    {
        if (!TryGetThread(threadName, out var thread) || thread is null)
        {
            return false;
        }

        thread.Pause();
        return true;
    }

    /// <summary>
    /// \if KO
    /// <para>이름이 일치하는 작업자 스레드를 재개합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Resumes the worker thread whose name matches.</para>
    /// \endif
    /// </summary>
    /// <param name="threadName">
    /// \if KO
    /// <para>재개할 작업자 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The worker name to resume.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>작업자를 찾아 재개했으면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when the worker was found and resumed.</para>
    /// \endif
    /// </returns>
    public bool Resume(string threadName)
    {
        if (!TryGetThread(threadName, out var thread) || thread is null)
        {
            return false;
        }

        thread.Resume();
        return true;
    }

    /// <summary>
    /// \if KO
    /// <para>등록된 모든 작업자 스레드를 일시 정지합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Pauses all registered worker threads.</para>
    /// \endif
    /// </summary>
    public void PauseAll()
    {
        foreach (var thread in GetThreads())
        {
            thread.Pause();
        }
    }

    /// <summary>
    /// \if KO
    /// <para>등록된 모든 작업자 스레드를 재개합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Resumes all registered worker threads.</para>
    /// \endif
    /// </summary>
    public void ResumeAll()
    {
        foreach (var thread in GetThreads())
        {
            thread.Resume();
        }
    }

    /// <summary>
    /// \if KO
    /// <para>대소문자를 구분하는 이름으로 등록된 작업자 스레드를 찾습니다.</para>
    /// \endif
    /// \if EN
    /// <para>Finds a registered worker thread by its case-sensitive name.</para>
    /// \endif
    /// </summary>
    /// <param name="threadName">
    /// \if KO
    /// <para>찾을 작업자 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The worker name to find.</para>
    /// \endif
    /// </param>
    /// <param name="thread">
    /// \if KO
    /// <para>성공 시 찾은 작업자를 받습니다.</para>
    /// \endif
    /// \if EN
    /// <para>Receives the found worker on success.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>유효한 이름의 작업자를 찾았으면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when a worker with a valid matching name is found.</para>
    /// \endif
    /// </returns>
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

    /// <summary>
    /// \if KO
    /// <para>등록된 작업자 스레드의 스레드 안전한 배열 스냅샷을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets a thread-safe array snapshot of registered worker threads.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>등록된 작업자 스레드의 스냅샷입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A snapshot of registered worker threads.</para>
    /// \endif
    /// </returns>
    public IReadOnlyList<IDreamineThread> GetThreads()
    {
        lock (_syncRoot)
        {
            return _threads.ToArray();
        }
    }

    /// <summary>
    /// \if KO
    /// <para>모든 등록된 작업자에서 상태 스냅샷을 생성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates state snapshots from all registered workers.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>작업자 상태 정보 배열입니다.</para>
    /// \endif
    /// \if EN
    /// <para>An array of worker-state information.</para>
    /// \endif
    /// </returns>
    public IReadOnlyList<DreamineThreadInfo> GetThreadInfos()
    {
        lock (_syncRoot)
        {
            return _threads
                .Select(thread => thread.GetInfo())
                .ToArray();
        }
    }

    /// <summary>
    /// \if KO
    /// <para>모든 작업자를 동기 중지·정리하고 코어 할당을 반환합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Synchronously stops and disposes all workers and returns their core assignments.</para>
    /// \endif
    /// </summary>
    /// <remarks>
    /// \if KO
    /// <para>모든 작업자의 join 대기가 완료될 때까지 호출 스레드를 차단할 수 있습니다.</para>
    /// \endif
    /// \if EN
    /// <para>This method can block the caller until join waits for all workers complete.</para>
    /// \endif
    /// </remarks>
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
    /// \if KO
    /// <para>오버플로 폴링 작업을 기존 작업자에 배치하거나 작업자가 없으면 대체 작업자를 생성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Routes an overflow polling job to an existing worker, or creates a fallback worker when none exists.</para>
    /// \endif
    /// </summary>
    /// <param name="job">
    /// \if KO
    /// <para>배치할 오버플로 폴링 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The overflow polling job to route.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>새 대체 작업자가 생성되면 잠금 밖에서 시작할 작업자이며 기존 작업자가 작업을 받으면 <see langword="null"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The worker to start outside the lock when a fallback worker is created; <see langword="null"/> when an existing worker absorbs the job.</para>
    /// \endif
    /// </returns>
    /// <remarks>
    /// \if KO
    /// <para>오버플로 폴링 작업을 기존 작업자에 배치하거나 작업자가 없으면 대체 작업자를 생성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Routes an overflow polling job to an existing worker, or creates a fallback worker when none exists.</para>
    /// \endif
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

    /// <summary>
    /// \if KO
    /// <para>스레드 관리자가 아직 정리되지 않았는지 확인합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Verifies that the thread manager has not been disposed.</para>
    /// \endif
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// \if KO
    /// <para>관리자가 이미 정리된 경우 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the manager has already been disposed.</para>
    /// \endif
    /// </exception>
    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(DreamineThreadManager));
        }
    }
}
