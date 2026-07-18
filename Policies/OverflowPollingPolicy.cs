using Dreamine.Threading.Interfaces;
using Dreamine.Threading.Models;

namespace Dreamine.Threading.Policies;

/// <summary>
/// \if KO
/// <para>오버플로 작업을 위한 폴링 간격 정책을 제공합니다.</para>
/// \endif
/// \if EN
/// <para>Provides a polling-interval policy for overflow jobs.</para>
/// \endif
/// </summary>
public sealed class OverflowPollingPolicy : IThreadCyclePolicy
{
    /// <summary>
    /// \if KO
    /// <para>구성된 오버플로 폴링 간격을 반환하며 음수는 100밀리초로 보정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Returns the configured overflow polling interval, normalizing negative values to 100 milliseconds.</para>
    /// \endif
    /// </summary>
    /// <param name="options">
    /// \if KO
    /// <para>오버플로 폴링 간격 설정입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The overflow polling interval settings.</para>
    /// \endif
    /// </param>
    /// <param name="assignment">
    /// \if KO
    /// <para>현재 코어 할당이며 계약 검증을 위해 필요합니다.</para>
    /// \endif
    /// \if EN
    /// <para>The current core assignment, required for contract validation.</para>
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
    /// <para>정규화된 오버플로 폴링 지연입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The normalized overflow polling delay.</para>
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

        return options.OverflowPollingIntervalMs < 0
            ? 100
            : options.OverflowPollingIntervalMs;
    }
}
