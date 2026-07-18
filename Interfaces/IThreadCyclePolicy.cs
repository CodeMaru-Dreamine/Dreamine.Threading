using Dreamine.Threading.Models;

namespace Dreamine.Threading.Interfaces;

/// <summary>
/// \if KO
/// <para>작업자 스레드 주기 사이의 지연을 결정하는 정책 계약을 정의합니다.</para>
/// \endif
/// \if EN
/// <para>Defines a policy contract that determines the delay between worker-thread cycles.</para>
/// \endif
/// </summary>
public interface IThreadCyclePolicy
{
    /// <summary>
    /// \if KO
    /// <para>다음 작업자 주기까지 기다릴 밀리초 지연을 계산합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Calculates the delay in milliseconds before the next worker cycle.</para>
    /// \endif
    /// </summary>
    /// <param name="options">
    /// \if KO
    /// <para>주기 설정을 포함하는 스레드 옵션입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The thread options containing cycle settings.</para>
    /// \endif
    /// </param>
    /// <param name="assignment">
    /// \if KO
    /// <para>현재 CPU 코어 할당입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The current CPU core assignment.</para>
    /// \endif
    /// </param>
    /// <param name="context">
    /// \if KO
    /// <para>현재 실행 시간과 CPU 사용률을 포함하는 주기 컨텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The current cycle context containing execution time and CPU usage.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>다음 주기 전 대기할 밀리초입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The number of milliseconds to wait before the next cycle.</para>
    /// \endif
    /// </returns>
    int GetDelayMs(
        DreamineThreadOptions options,
        DreamineThreadCoreAssignment assignment,
        DreamineThreadCycleContext context);
}
