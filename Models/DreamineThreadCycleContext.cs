namespace Dreamine.Threading.Models;

/// <summary>
/// \if KO
/// <para>Dreamine 스레드 주기의 런타임 컨텍스트 정보를 나타냅니다.</para>
/// \endif
/// \if EN
/// <para>Represents runtime context information for a Dreamine thread cycle.</para>
/// \endif
/// </summary>
public sealed class DreamineThreadCycleContext
{
    /// <summary>
    /// \if KO
    /// <para>작업자 스레드 이름을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the worker thread name.</para>
    /// \endif
    /// </summary>
    public string ThreadName { get; }

    /// <summary>
    /// \if KO
    /// <para>현재 누적 주기 수를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the current cumulative cycle count.</para>
    /// \endif
    /// </summary>
    public long CycleCount { get; }

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
    /// <para>할당된 CPU 코어 인덱스를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the assigned CPU core index.</para>
    /// \endif
    /// </summary>
    public int? CoreIndex { get; }

    /// <summary>
    /// \if KO
    /// <para>이 주기가 오버플로 폴링용인지 여부를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets whether this cycle is for overflow polling.</para>
    /// \endif
    /// </summary>
    public bool IsOverflowPolling { get; }

    /// <summary>
    /// \if KO
    /// <para>현재 주기 타임스탬프를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the current cycle timestamp.</para>
    /// \endif
    /// </summary>
    public DateTimeOffset Timestamp { get; }

    /// <summary>
    /// \if KO
    /// <para><see cref="T:Dreamine.Threading.Models.DreamineThreadCycleContext" /> 클래스의 새 인스턴스를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes a new instance of <see cref="T:Dreamine.Threading.Models.DreamineThreadCycleContext" />.</para>
    /// \endif
    /// </summary>
    /// <param name="threadName">
    /// \if KO
    /// <para>작업자 스레드 이름이며 비어 있으면 기본 이름을 사용합니다.</para>
    /// \endif
    /// \if EN
    /// <para>The worker-thread name; a default name is used when it is blank.</para>
    /// \endif
    /// </param>
    /// <param name="cycleCount">
    /// \if KO
    /// <para>현재 누적 주기 수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The current cumulative cycle count.</para>
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
    /// <param name="coreIndex">
    /// \if KO
    /// <para>할당된 CPU 코어 인덱스입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The assigned CPU core index.</para>
    /// \endif
    /// </param>
    /// <param name="isOverflowPolling">
    /// \if KO
    /// <para>오버플로 폴링 주기인지 여부입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Whether the cycle is for overflow polling.</para>
    /// \endif
    /// </param>
    /// <param name="timestamp">
    /// \if KO
    /// <para>현재 주기 타임스탬프입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The current cycle timestamp.</para>
    /// \endif
    /// </param>
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
