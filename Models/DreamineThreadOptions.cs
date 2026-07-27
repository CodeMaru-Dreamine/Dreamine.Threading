namespace Dreamine.Threading.Models;

/// <summary>
/// \if KO
/// <para>Dreamine 작업자 스레드 생성 옵션을 나타냅니다.</para>
/// \endif
/// \if EN
/// <para>Represents options used to create a Dreamine worker thread.</para>
/// \endif
/// </summary>
public sealed class DreamineThreadOptions
{
    /// <summary>
    /// \if KO
    /// <para>작업자 스레드 이름을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the worker-thread name.</para>
    /// \endif
    /// </summary>
    public string Name { get; set; } = "DreamineThread";

    /// <summary>
    /// \if KO
    /// <para>작업자 스레드 우선순위를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the worker-thread priority.</para>
    /// \endif
    /// </summary>
    public DreamineThreadPriority Priority { get; set; } = DreamineThreadPriority.Normal;

    /// <summary>
    /// \if KO
    /// <para>밀리초 단위 기본 실행 주기 간격을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the base cycle interval in milliseconds.</para>
    /// \endif
    /// </summary>
    public int IntervalMs { get; set; } = 10;

    /// <summary>
    /// \if KO
    /// <para>CPU 코어 할당 모드를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the CPU core-assignment mode.</para>
    /// \endif
    /// </summary>
    public DreamineThreadCoreMode CoreMode { get; set; } = DreamineThreadCoreMode.Auto;

    /// <summary>
    /// \if KO
    /// <para>수동으로 할당할 CPU 코어 인덱스를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the manually assigned CPU core index.</para>
    /// \endif
    /// </summary>
    public int? CoreIndex { get; set; }

    /// <summary>
    /// \if KO
    /// <para>자동 모드에서 CPU 코어당 최대 전용 작업자 스레드 수를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the maximum number of dedicated worker threads per CPU core in automatic mode.</para>
    /// \endif
    /// </summary>
    public int AutoThreadsPerCore { get; set; } = 2;

    /// <summary>
    /// \if KO
    /// <para>작업이 오버플로 폴링으로 할당될 때 사용할 폴링 간격을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the polling interval used when a job is assigned to overflow polling.</para>
    /// \endif
    /// </summary>
    public int OverflowPollingIntervalMs { get; set; } = 100;

    /// <summary>
    /// \if KO
    /// <para>생성 직후 작업자를 시작할지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether the worker starts immediately after creation.</para>
    /// \endif
    /// </summary>
    public bool AutoStart { get; set; } = true;

    /// <summary>
    /// \if KO
    /// <para>고정밀 타이머 해상도를 요청할지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether high-precision timer resolution is requested.</para>
    /// \endif
    /// </summary>
    public bool UseHighPrecisionTimer { get; set; } = true;

    /// <summary>
    /// \if KO
    /// <para>간격이 0일 때 적응형 CPU 지연을 사용할지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether adaptive CPU delay is enabled when the interval is zero.</para>
    /// \endif
    /// </summary>
    public bool UseAdaptiveCpuDelay { get; set; } = true;

    /// <summary>
    /// \if KO
    /// <para>간격이 0일 때 작업자 스레드가 실행권을 양보할지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether the worker yields when the interval is zero.</para>
    /// \endif
    /// </summary>
    public bool YieldWhenIntervalIsZero { get; set; } = true;

    /// <summary>
    /// \if KO
    /// <para>중지 작업이 작업자 스레드 종료를 기다릴 제한 시간을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets how long stop operations wait for the worker thread to exit.</para>
    /// \endif
    /// </summary>
    public TimeSpan StopTimeout { get; set; } = TimeSpan.FromSeconds(2);

    /// <summary>
    /// \if KO
    /// <para>잘못된 이름·간격·코어당 스레드 수·중지 제한 시간을 기본값으로 보정한 복사본을 생성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates a copy with invalid names, intervals, threads-per-core, and stop timeouts normalized to defaults.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>정규화된 새 스레드 옵션입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A new normalized thread-options instance.</para>
    /// \endif
    /// </returns>
    public DreamineThreadOptions Normalize()
    {
        return new DreamineThreadOptions
        {
            Name = string.IsNullOrWhiteSpace(Name) ? "DreamineThread" : Name,
            Priority = Priority,
            IntervalMs = IntervalMs < 0 ? 10 : IntervalMs,
            CoreMode = CoreMode,
            CoreIndex = CoreIndex,
            AutoThreadsPerCore = AutoThreadsPerCore <= 0 ? 2 : AutoThreadsPerCore,
            OverflowPollingIntervalMs = OverflowPollingIntervalMs < 0 ? 100 : OverflowPollingIntervalMs,
            AutoStart = AutoStart,
            UseHighPrecisionTimer = UseHighPrecisionTimer,
            YieldWhenIntervalIsZero = YieldWhenIntervalIsZero,
            UseAdaptiveCpuDelay = UseAdaptiveCpuDelay,
            StopTimeout = StopTimeout <= TimeSpan.Zero ? TimeSpan.FromSeconds(2) : StopTimeout
        };
    }
}
