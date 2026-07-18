namespace Dreamine.Threading.Models;

/// <summary>
/// \if KO
/// <para>Dreamine 스레드의 우선순위 수준을 정의합니다.</para>
/// \endif
/// \if EN
/// <para>Defines the priority level of a Dreamine thread.</para>
/// \endif
/// </summary>
public enum DreamineThreadPriority
{
    /// <summary>
    /// \if KO
    /// <para>백그라운드 또는 모니터링 작업을 위한 낮은 우선순위입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Low priority for background or monitoring work.</para>
    /// \endif
    /// </summary>
    Low = 0,

    /// <summary>
    /// \if KO
    /// <para>일반 작업을 위한 보통 우선순위입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Normal priority for standard work.</para>
    /// \endif
    /// </summary>
    Normal = 1,

    /// <summary>
    /// \if KO
    /// <para>시간에 민감한 작업을 위한 높은 우선순위입니다.</para>
    /// \endif
    /// \if EN
    /// <para>High priority for timing-sensitive work.</para>
    /// \endif
    /// </summary>
    High = 2
}
