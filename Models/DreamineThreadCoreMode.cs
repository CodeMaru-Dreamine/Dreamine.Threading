namespace Dreamine.Threading.Models;

/// <summary>
/// \if KO
/// <para>Dreamine 스레드가 CPU 코어에 할당되는 방식을 정의합니다.</para>
/// \endif
/// \if EN
/// <para>Defines how a Dreamine thread is assigned to CPU cores.</para>
/// \endif
/// </summary>
public enum DreamineThreadCoreMode
{
    /// <summary>
    /// \if KO
    /// <para>CPU 선호도를 적용하지 않습니다.</para>
    /// \endif
    /// \if EN
    /// <para>No CPU affinity is applied.</para>
    /// \endif
    /// </summary>
    None = 0,

    /// <summary>
    /// \if KO
    /// <para>CPU 코어 할당을 자동으로 선택합니다.</para>
    /// \endif
    /// \if EN
    /// <para>CPU core assignment is selected automatically.</para>
    /// \endif
    /// </summary>
    Auto = 1,

    /// <summary>
    /// \if KO
    /// <para>CPU 코어 할당을 수동으로 선택합니다.</para>
    /// \endif
    /// \if EN
    /// <para>CPU core assignment is selected manually.</para>
    /// \endif
    /// </summary>
    Manual = 2
}
