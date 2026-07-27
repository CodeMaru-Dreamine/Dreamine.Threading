namespace Dreamine.Threading.Interfaces;

/// <summary>
/// \if KO
/// <para>시스템 타이머 해상도를 제어하는 플랫폼 서비스 계약을 정의합니다.</para>
/// \endif
/// \if EN
/// <para>Defines a platform-service contract that controls system timer resolution.</para>
/// \endif
/// </summary>
public interface ITimerResolutionService
{
    /// <summary>
    /// \if KO
    /// <para>고정밀 타이머 해상도 사용을 시작합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Begins using high-precision timer resolution.</para>
    /// \endif
    /// </summary>
    void Begin();

    /// <summary>
    /// \if KO
    /// <para>고정밀 타이머 해상도 사용을 종료합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Ends use of high-precision timer resolution.</para>
    /// \endif
    /// </summary>
    void End();
}
