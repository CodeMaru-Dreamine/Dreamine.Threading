namespace Dreamine.Threading.Models;

/// <summary>
/// \if KO
/// <para>Dreamine 작업자 스레드 상태의 불변 스냅샷을 나타냅니다.</para>
/// \endif
/// \if EN
/// <para>Represents an immutable snapshot of a Dreamine worker-thread state.</para>
/// \endif
/// </summary>
public sealed class DreamineThreadInfo
{
    /// <summary>
    /// \if KO
    /// <para>작업자 스레드 이름을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the worker-thread name.</para>
    /// \endif
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// \if KO
    /// <para>작업자 스레드 상태를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the worker-thread status.</para>
    /// \endif
    /// </summary>
    public DreamineThreadStatus Status { get; }

    /// <summary>
    /// \if KO
    /// <para>작업자 스레드 우선순위를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the worker-thread priority.</para>
    /// \endif
    /// </summary>
    public DreamineThreadPriority Priority { get; }

    /// <summary>
    /// \if KO
    /// <para>밀리초 단위 작업자 실행 간격을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the worker-thread interval in milliseconds.</para>
    /// \endif
    /// </summary>
    public int IntervalMs { get; }

    /// <summary>
    /// \if KO
    /// <para>할당된 CPU 코어 인덱스를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the assigned CPU core index.</para>
    /// \endif
    /// </summary>
    public int? CoreIndex { get; }

    /// <summary>
    /// \if KO
    /// <para>CPU 선호도가 활성화되었는지 여부를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets whether CPU affinity is enabled.</para>
    /// \endif
    /// </summary>
    public bool UseAffinity { get; }

    /// <summary>
    /// \if KO
    /// <para>작업자에 할당된 작업 수를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the number of jobs assigned to the worker.</para>
    /// \endif
    /// </summary>
    public int JobCount { get; }

    /// <summary>
    /// \if KO
    /// <para>완료된 실행 주기 수를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the number of completed cycles.</para>
    /// \endif
    /// </summary>
    public long CycleCount { get; }

    /// <summary>
    /// \if KO
    /// <para>마지막 시작 시각을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the most recent start time.</para>
    /// \endif
    /// </summary>
    public DateTimeOffset? StartedAt { get; }

    /// <summary>
    /// \if KO
    /// <para>마지막 중지 시각을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the most recent stop time.</para>
    /// \endif
    /// </summary>
    public DateTimeOffset? StoppedAt { get; }

    /// <summary>
    /// \if KO
    /// <para>마지막 예외 메시지를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the most recent exception message.</para>
    /// \endif
    /// </summary>
    public string? LastErrorMessage { get; }

    /// <summary>
    /// \if KO
    /// <para><see cref="T:Dreamine.Threading.Models.DreamineThreadInfo" /> 클래스의 새 스냅샷을 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes a new snapshot instance of <see cref="T:Dreamine.Threading.Models.DreamineThreadInfo" />.</para>
    /// \endif
    /// </summary>
    /// <param name="name">
    /// \if KO
    /// <para>작업자 스레드 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The worker-thread name.</para>
    /// \endif
    /// </param>
    /// <param name="status">
    /// \if KO
    /// <para>현재 수명 주기 상태입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The current lifecycle status.</para>
    /// \endif
    /// </param>
    /// <param name="priority">
    /// \if KO
    /// <para>스레드 우선순위입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The thread priority.</para>
    /// \endif
    /// </param>
    /// <param name="intervalMs">
    /// \if KO
    /// <para>밀리초 단위 실행 간격입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The execution interval in milliseconds.</para>
    /// \endif
    /// </param>
    /// <param name="coreIndex">
    /// \if KO
    /// <para>할당된 CPU 코어 인덱스입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The assigned CPU core index.</para>
    /// \endif
    /// </param>
    /// <param name="useAffinity">
    /// \if KO
    /// <para>CPU 선호도 활성화 여부입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Whether CPU affinity is enabled.</para>
    /// \endif
    /// </param>
    /// <param name="jobCount">
    /// \if KO
    /// <para>할당된 작업 수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The number of assigned jobs.</para>
    /// \endif
    /// </param>
    /// <param name="cycleCount">
    /// \if KO
    /// <para>완료된 주기 수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The number of completed cycles.</para>
    /// \endif
    /// </param>
    /// <param name="startedAt">
    /// \if KO
    /// <para>마지막 시작 시각입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The most recent start time.</para>
    /// \endif
    /// </param>
    /// <param name="stoppedAt">
    /// \if KO
    /// <para>마지막 중지 시각입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The most recent stop time.</para>
    /// \endif
    /// </param>
    /// <param name="lastErrorMessage">
    /// \if KO
    /// <para>마지막 오류 메시지입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The most recent error message.</para>
    /// \endif
    /// </param>
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
