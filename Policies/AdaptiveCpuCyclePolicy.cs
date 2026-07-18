using Dreamine.Threading.Interfaces;
using Dreamine.Threading.Models;

namespace Dreamine.Threading.Policies;

/// <summary>
/// \if KO
/// <para>CPU 사용률에 따라 실행 지연을 늘리는 적응형 주기 정책을 제공합니다.</para>
/// \endif
/// \if EN
/// <para>Provides an adaptive cycle policy that increases execution delay based on CPU usage.</para>
/// \endif
/// </summary>
public sealed class AdaptiveCpuCyclePolicy : IThreadCyclePolicy
{
    /// <summary>
    /// \if KO
    /// <para>cpu Usage Provider 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the cpu usage provider value.</para>
    /// \endif
    /// </summary>
    private readonly ICpuUsageProvider _cpuUsageProvider;

    /// <summary>
    /// \if KO
    /// <para><see cref="T:Dreamine.Threading.Policies.AdaptiveCpuCyclePolicy" /> 클래스의 새 인스턴스를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes a new instance of <see cref="T:Dreamine.Threading.Policies.AdaptiveCpuCyclePolicy" />.</para>
    /// \endif
    /// </summary>
    /// <param name="cpuUsageProvider">
    /// \if KO
    /// <para>전체 CPU 사용률을 제공할 서비스입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The service that provides total CPU usage.</para>
    /// \endif
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// \if KO
    /// <para><paramref name="cpuUsageProvider"/>가 <see langword="null"/>일 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when <paramref name="cpuUsageProvider"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
    public AdaptiveCpuCyclePolicy(ICpuUsageProvider cpuUsageProvider)
    {
        _cpuUsageProvider = cpuUsageProvider ?? throw new ArgumentNullException(nameof(cpuUsageProvider));
    }

    /// <summary>
    /// \if KO
    /// <para>오버플로·고정 간격·CPU 사용률 규칙에 따라 다음 주기 지연을 계산합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Calculates the next cycle delay from overflow, fixed-interval, and CPU-usage rules.</para>
    /// \endif
    /// </summary>
    /// <param name="options">
    /// \if KO
    /// <para>간격 및 적응형 지연 설정입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The interval and adaptive-delay settings.</para>
    /// \endif
    /// </param>
    /// <param name="assignment">
    /// \if KO
    /// <para>현재 코어 및 오버플로 할당입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The current core and overflow assignment.</para>
    /// \endif
    /// </param>
    /// <param name="context">
    /// \if KO
    /// <para>현재 주기 컨텍스트이며 계약 검증을 위해 필요합니다.</para>
    /// \endif
    /// \if EN
    /// <para>The current cycle context, required for contract validation.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>다음 주기 전 대기할 0~5밀리초 또는 구성된 간격입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A zero-to-five millisecond adaptive delay or the configured interval.</para>
    /// \endif
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// \if KO
    /// <para>인수 중 하나가 <see langword="null"/>일 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when any argument is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
    public int GetDelayMs(
        DreamineThreadOptions options,
        DreamineThreadCoreAssignment assignment,
        DreamineThreadCycleContext context)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(assignment);
        ArgumentNullException.ThrowIfNull(context);

        if (assignment.IsOverflowPolling)
        {
            return options.OverflowPollingIntervalMs < 0
                ? 100
                : options.OverflowPollingIntervalMs;
        }

        if (options.IntervalMs > 0)
        {
            return options.IntervalMs;
        }

        if (options.IntervalMs < 0)
        {
            return 10;
        }

        if (!options.UseAdaptiveCpuDelay)
        {
            return 0;
        }

        var cpuUsage = _cpuUsageProvider.GetTotalCpuUsagePercent();

        if (cpuUsage >= 70)
        {
            return 5;
        }

        if (cpuUsage >= 50)
        {
            return 3;
        }

        if (cpuUsage >= 30)
        {
            return 1;
        }

        return 0;
    }
}
