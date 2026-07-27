using Dreamine.Threading.Interfaces;
using Dreamine.Threading.Models;

namespace Dreamine.Threading.Policies;

/// <summary>
/// \if KO
/// <para>Dreamine 작업자 스레드를 위한 고정 간격 주기 정책을 제공합니다.</para>
/// \endif
/// \if EN
/// <para>Provides a fixed-interval cycle policy for Dreamine worker threads.</para>
/// \endif
/// </summary>
public sealed class FixedIntervalCyclePolicy : IThreadCyclePolicy
{
    /// <summary>
    /// \if KO
    /// <para>일반 작업은 기본 간격, 오버플로 작업은 오버플로 간격을 사용해 지연을 계산합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Calculates delay using the base interval for normal work and the overflow interval for overflow work.</para>
    /// \endif
    /// </summary>
    /// <param name="options">
    /// \if KO
    /// <para>기본 및 오버플로 간격 설정입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The base and overflow interval settings.</para>
    /// \endif
    /// </param>
    /// <param name="assignment">
    /// \if KO
    /// <para>현재 오버플로 여부를 포함하는 코어 할당입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The core assignment containing current overflow status.</para>
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
    /// <para>정규화된 밀리초 단위 지연입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The normalized delay in milliseconds.</para>
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

        return options.IntervalMs < 0
            ? 10
            : options.IntervalMs;
    }
}
