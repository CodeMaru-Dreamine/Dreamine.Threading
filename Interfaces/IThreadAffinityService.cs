namespace Dreamine.Threading.Interfaces;

/// <summary>
/// \if KO
/// <para>현재 스레드에 CPU 선호도를 적용하는 플랫폼 서비스 계약을 정의합니다.</para>
/// \endif
/// \if EN
/// <para>Defines a platform-service contract that applies CPU affinity to the current thread.</para>
/// \endif
/// </summary>
public interface IThreadAffinityService
{
    /// <summary>
    /// \if KO
    /// <para>현재 스레드를 지정한 CPU 코어에 고정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Applies affinity for the specified CPU core to the current thread.</para>
    /// \endif
    /// </summary>
    /// <param name="coreIndex">
    /// \if KO
    /// <para>적용할 0부터 시작하는 CPU 코어 인덱스입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The zero-based CPU core index to apply.</para>
    /// \endif
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// \if KO
    /// <para>코어 인덱스가 유효하지 않으면 구현체가 발생시킬 수 있습니다.</para>
    /// \endif
    /// \if EN
    /// <para>May be thrown by an implementation when the core index is invalid.</para>
    /// \endif
    /// </exception>
    /// <exception cref="PlatformNotSupportedException">
    /// \if KO
    /// <para>플랫폼이 스레드 선호도를 지원하지 않으면 구현체가 발생시킬 수 있습니다.</para>
    /// \endif
    /// \if EN
    /// <para>May be thrown by an implementation when thread affinity is unsupported.</para>
    /// \endif
    /// </exception>
    void ApplyToCurrentThread(int coreIndex);

    /// <summary>
    /// \if KO
    /// <para>플랫폼이 지원하는 경우 현재 스레드의 CPU 선호도를 해제합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Clears CPU affinity from the current thread when supported.</para>
    /// \endif
    /// </summary>
    void ClearCurrentThreadAffinity();
}
