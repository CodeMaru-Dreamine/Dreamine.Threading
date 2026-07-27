using Dreamine.Logging.Interfaces;
using Dreamine.Threading.Interfaces;
using Dreamine.Threading.Models;

namespace Dreamine.Threading.Services;

/// <summary>
/// \if KO
/// <para>기본 Dreamine 작업자 스레드 구현을 제공합니다.</para>
/// \endif
/// \if EN
/// <para>Provides the default Dreamine worker-thread implementation.</para>
/// \endif
/// </summary>
public sealed class DreamineThread : IDreamineThread
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
    /// <para>jobs 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the jobs value.</para>
    /// \endif
    /// </summary>
    private readonly List<IDreamineThreadJob> _jobs = new();
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
    /// <para>pause Event 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the pause event value.</para>
    /// \endif
    /// </summary>
    private readonly ManualResetEventSlim _pauseEvent = new(true);

    /// <summary>
    /// \if KO
    /// <para>thread 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the thread value.</para>
    /// \endif
    /// </summary>
    private Thread? _thread;
    /// <summary>
    /// \if KO
    /// <para>cancellation Token Source 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the cancellation token source value.</para>
    /// \endif
    /// </summary>
    private CancellationTokenSource? _cancellationTokenSource;
    /// <summary>
    /// \if KO
    /// <para>cycle Count 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the cycle count value.</para>
    /// \endif
    /// </summary>
    private long _cycleCount;
    /// <summary>
    /// \if KO
    /// <para>started At 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the started at value.</para>
    /// \endif
    /// </summary>
    private DateTimeOffset? _startedAt;
    /// <summary>
    /// \if KO
    /// <para>stopped At 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the stopped at value.</para>
    /// \endif
    /// </summary>
    private DateTimeOffset? _stoppedAt;
    /// <summary>
    /// \if KO
    /// <para>last Error Message 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the last error message value.</para>
    /// \endif
    /// </summary>
    private string? _lastErrorMessage;
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
    /// <para>정규화된 작업자 스레드 이름을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the normalized worker-thread name.</para>
    /// \endif
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// \if KO
    /// <para>정규화된 작업자 스레드 옵션을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the normalized worker-thread options.</para>
    /// \endif
    /// </summary>
    public DreamineThreadOptions Options { get; }

    /// <summary>
    /// \if KO
    /// <para>작업자 스레드의 CPU 코어 할당을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the worker thread's CPU core assignment.</para>
    /// \endif
    /// </summary>
    public DreamineThreadCoreAssignment CoreAssignment { get; }

    /// <summary>
    /// \if KO
    /// <para>status 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the status value.</para>
    /// \endif
    /// </summary>
    private volatile DreamineThreadStatus _status = DreamineThreadStatus.Created;

    /// <summary>
    /// \if KO
    /// <para>현재 작업자 스레드 수명 주기 상태를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the current worker-thread lifecycle status.</para>
    /// \endif
    /// </summary>
    public DreamineThreadStatus Status
    {
        get => _status;
        private set => _status = value;
    }

    /// <summary>
    /// \if KO
    /// <para>이 작업자에 할당된 작업 수를 스레드 안전하게 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the number of jobs assigned to this worker in a thread-safe manner.</para>
    /// \endif
    /// </summary>
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
    /// \if KO
    /// <para>지정한 옵션, 코어 할당 및 실행 서비스를 사용해 <see cref="T:Dreamine.Threading.Services.DreamineThread" /> 클래스의 새 인스턴스를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes a new instance of <see cref="T:Dreamine.Threading.Services.DreamineThread" /> using the specified options, core assignment, and execution services.</para>
    /// \endif
    /// </summary>
    /// <param name="options">
    /// \if KO
    /// <para>정규화할 작업자 스레드 옵션입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The worker-thread options to normalize.</para>
    /// \endif
    /// </param>
    /// <param name="coreAssignment">
    /// \if KO
    /// <para>적용할 CPU 코어 할당입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The CPU core assignment to apply.</para>
    /// \endif
    /// </param>
    /// <param name="cyclePolicy">
    /// \if KO
    /// <para>각 실행 주기 뒤의 지연을 결정할 정책입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The policy that determines delay after each execution cycle.</para>
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
    /// <para><paramref name="options"/>, <paramref name="coreAssignment"/> 또는 <paramref name="cyclePolicy"/>가 <see langword="null"/>일 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when <paramref name="options"/>, <paramref name="coreAssignment"/>, or <paramref name="cyclePolicy"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
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

    /// <summary>
    /// \if KO
    /// <para>실행 주기에 참여할 작업을 스레드 안전하게 추가합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Adds a job to participate in execution cycles in a thread-safe manner.</para>
    /// \endif
    /// </summary>
    /// <param name="job">
    /// \if KO
    /// <para>추가할 스레드 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The thread job to add.</para>
    /// \endif
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// \if KO
    /// <para><paramref name="job"/>이 <see langword="null"/>일 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when <paramref name="job"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// \if KO
    /// <para>작업자 스레드가 이미 정리된 경우 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the worker thread has already been disposed.</para>
    /// \endif
    /// </exception>
    public void AddJob(IDreamineThreadJob job)
    {
        ArgumentNullException.ThrowIfNull(job);

        lock (_syncRoot)
        {
            ThrowIfDisposed();
            _jobs.Add(job);
        }
    }

    /// <summary>
    /// \if KO
    /// <para>백그라운드 작업자 스레드를 생성하고 실행을 시작합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates the background worker thread and starts execution.</para>
    /// \endif
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// \if KO
    /// <para>작업자 스레드가 이미 정리된 경우 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the worker thread has already been disposed.</para>
    /// \endif
    /// </exception>
    /// <exception cref="ThreadStateException">
    /// \if KO
    /// <para>생성된 시스템 스레드를 시작할 수 없는 상태일 때 발생할 수 있습니다.</para>
    /// \endif
    /// \if EN
    /// <para>May be thrown when the created system thread cannot be started in its current state.</para>
    /// \endif
    /// </exception>
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

    /// <summary>
    /// \if KO
    /// <para>실행 중인 작업자 스레드의 주기 진행을 일시 정지합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Pauses cycle progression for a running worker thread.</para>
    /// \endif
    /// </summary>
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

    /// <summary>
    /// \if KO
    /// <para>일시 정지된 작업자 스레드의 주기 진행을 재개합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Resumes cycle progression for a paused worker thread.</para>
    /// \endif
    /// </summary>
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

    /// <summary>
    /// \if KO
    /// <para>작업자 스레드 중지를 요청하고 종료 대기를 동기적으로 차단합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Requests worker-thread shutdown and blocks synchronously during the join wait.</para>
    /// \endif
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// \if KO
    /// <para>비동기 중지 작업을 동기적으로 완료할 수 없을 때 발생할 수 있습니다.</para>
    /// \endif
    /// \if EN
    /// <para>May be thrown when the asynchronous stop operation cannot be completed synchronously.</para>
    /// \endif
    /// </exception>
    [Obsolete("Stop() blocks the calling thread and risks deadlock on a SynchronizationContext. Use StopAsync() instead. / 호출 스레드를 블로킹하며 SynchronizationContext 환경에서 데드락 위험이 있습니다. StopAsync()를 사용하세요.")]
    public void Stop()
    {
        StopAsync().AsTask().GetAwaiter().GetResult();
    }

    /// <summary>
    /// \if KO
    /// <para>취소를 요청하고 구성된 제한 시간 동안 작업자 스레드 종료를 비동기로 기다립니다.</para>
    /// \endif
    /// \if EN
    /// <para>Requests cancellation and asynchronously waits for worker-thread exit for the configured timeout.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>중지와 상태 정리를 나타내는 값 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A value task representing shutdown and state cleanup.</para>
    /// \endif
    /// </returns>
    /// <remarks>
    /// \if KO
    /// <para>제한 시간 내 종료 여부와 관계없이 내부 스레드 참조를 지우고 상태를 중지됨으로 갱신합니다.</para>
    /// \endif
    /// \if EN
    /// <para>The internal thread reference is cleared and status is changed to stopped even if the join timeout expires.</para>
    /// \endif
    /// </remarks>
    public async ValueTask StopAsync()
    {
        Thread? threadToJoin;
        CancellationTokenSource? cancellationSource;

        lock (_syncRoot)
        {
            if (Status is DreamineThreadStatus.Stopped or DreamineThreadStatus.Disposed)
            {
                return;
            }

            Status = DreamineThreadStatus.Stopping;
            _pauseEvent.Set();

            cancellationSource = _cancellationTokenSource;
            threadToJoin = _thread;
        }

        if (cancellationSource is not null)
            await cancellationSource.CancelAsync().ConfigureAwait(false);

        if (threadToJoin is not null && threadToJoin.IsAlive)
        {
            await Task.Run(
                () => threadToJoin.Join(Options.StopTimeout),
                CancellationToken.None)
                .ConfigureAwait(false);
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

    /// <summary>
    /// \if KO
    /// <para>현재 작업자 상태와 실행 통계를 불변 스냅샷으로 반환합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Returns current worker state and execution statistics as an immutable snapshot.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>현재 <see cref="DreamineThreadInfo"/> 스냅샷입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The current <see cref="DreamineThreadInfo"/> snapshot.</para>
    /// \endif
    /// </returns>
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

    /// <summary>
    /// \if KO
    /// <para>작업자 스레드를 동기적으로 중지하고 대기 핸들을 정리합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stops the worker thread synchronously and disposes its wait handle.</para>
    /// \endif
    /// </summary>
    /// <remarks>
    /// \if KO
    /// <para>이 메서드는 중지의 join 대기 동안 호출 스레드를 차단할 수 있습니다.</para>
    /// \endif
    /// \if EN
    /// <para>This method can block the caller during the stop join wait.</para>
    /// \endif
    /// </remarks>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        StopAsync().AsTask().GetAwaiter().GetResult();

        _pauseEvent.Dispose();
        _disposed = true;
        Status = DreamineThreadStatus.Disposed;
    }

    /// <summary>
    /// \if KO
    /// <para>작업자 스레드에서 타이머·선호도·작업 실행·주기 지연을 관리하는 주 루프를 실행합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Runs the worker-thread main loop that manages timer resolution, affinity, jobs, and cycle delay.</para>
    /// \endif
    /// </summary>
    /// <param name="cancellationToken">
    /// \if KO
    /// <para>주 루프와 일시 정지 대기를 중지하는 토큰입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A token that stops the main loop and pause wait.</para>
    /// \endif
    /// </param>
    /// <remarks>
    /// \if KO
    /// <para>비취소 예외는 내부에서 기록되어 상태가 <see cref="DreamineThreadStatus.Faulted"/>로 변경되며 호출자에게 전파되지 않습니다.</para>
    /// \endif
    /// \if EN
    /// <para>Non-cancellation exceptions are recorded internally, change status to <see cref="DreamineThreadStatus.Faulted"/>, and are not propagated to the caller.</para>
    /// \endif
    /// </remarks>
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

    /// <summary>
    /// \if KO
    /// <para>현재 시각에 실행 기한이 된 작업을 등록 순서대로 동기 실행합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Executes jobs due at the current time synchronously in registration order.</para>
    /// \endif
    /// </summary>
    /// <param name="cancellationToken">
    /// \if KO
    /// <para>각 작업 실행에 전달할 취소 토큰입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The cancellation token passed to each job execution.</para>
    /// \endif
    /// </param>
    /// <exception cref="OperationCanceledException">
    /// \if KO
    /// <para>실행 중인 작업이 취소를 보고하면 다시 전파됩니다.</para>
    /// \endif
    /// \if EN
    /// <para>Rethrown when an executing job reports cancellation.</para>
    /// \endif
    /// </exception>
    /// <remarks>
    /// \if KO
    /// <para>개별 작업의 비취소 예외는 기록한 뒤 다음 작업을 계속 실행합니다.</para>
    /// \endif
    /// \if EN
    /// <para>A non-cancellation exception from an individual job is logged and execution continues with the next job.</para>
    /// \endif
    /// </remarks>
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
                // Jobs run on a dedicated worker thread. Blocking here is
                // intentional so jobs preserve registration order and the cycle
                // policy observes completed work before calculating the next delay.
                job.ExecuteAsync(cancellationToken).GetAwaiter().GetResult();
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

    /// <summary>
    /// \if KO
    /// <para>Dreamine 우선순위를 시스템 <see cref="ThreadPriority"/> 값으로 매핑합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Maps a Dreamine priority to a system <see cref="ThreadPriority"/> value.</para>
    /// \endif
    /// </summary>
    /// <param name="priority">
    /// \if KO
    /// <para>매핑할 Dreamine 스레드 우선순위입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The Dreamine thread priority to map.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>대응하는 시스템 스레드 우선순위입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The corresponding system thread priority.</para>
    /// \endif
    /// </returns>
    private static ThreadPriority MapPriority(DreamineThreadPriority priority)
    {
        return priority switch
        {
            DreamineThreadPriority.High => ThreadPriority.AboveNormal,
            DreamineThreadPriority.Low => ThreadPriority.BelowNormal,
            _ => ThreadPriority.Normal
        };
    }

    /// <summary>
    /// \if KO
    /// <para>작업자 스레드가 아직 정리되지 않았는지 확인합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Verifies that the worker thread has not been disposed.</para>
    /// \endif
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// \if KO
    /// <para>작업자 스레드가 이미 정리된 경우 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the worker thread has already been disposed.</para>
    /// \endif
    /// </exception>
    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(DreamineThread));
        }
    }
}
