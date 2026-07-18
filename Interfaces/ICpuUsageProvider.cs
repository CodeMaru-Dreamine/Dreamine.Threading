namespace Dreamine.Threading.Interfaces;

/// <summary>
/// \if KO
/// <para>CPU 사용률 정보를 제공하는 서비스 계약을 정의합니다.</para>
/// \endif
/// \if EN
/// <para>Defines a service contract that provides CPU usage information.</para>
/// \endif
/// </summary>
public interface ICpuUsageProvider
{
    /// <summary>
    /// \if KO
    /// <para>전체 CPU 사용률을 백분율로 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets total CPU usage as a percentage.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>전체 CPU 사용률 백분율입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The total CPU usage percentage.</para>
    /// \endif
    /// </returns>
    double GetTotalCpuUsagePercent();
}
