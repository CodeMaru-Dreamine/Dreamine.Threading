namespace Dreamine.Threading.Interfaces;

/// <summary>
/// \if KO
/// <para>스레딩 서비스에 CPU 토폴로지 정보를 제공하는 계약을 정의합니다.</para>
/// \endif
/// \if EN
/// <para>Defines a contract that provides CPU topology information to threading services.</para>
/// \endif
/// </summary>
public interface ICpuInfoProvider
{
    /// <summary>
    /// \if KO
    /// <para>사용 가능한 논리 프로세서 수를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the available logical processor count.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>논리 프로세서 수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The logical processor count.</para>
    /// \endif
    /// </returns>
    int GetLogicalProcessorCount();

    /// <summary>
    /// \if KO
    /// <para>지정한 CPU 코어 인덱스가 유효한지 확인합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Determines whether the specified CPU core index is valid.</para>
    /// \endif
    /// </summary>
    /// <param name="coreIndex">
    /// \if KO
    /// <para>검사할 0부터 시작하는 CPU 코어 인덱스입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The zero-based CPU core index to validate.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>유효하면 <see langword="true"/>, 아니면 <see langword="false"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when valid; otherwise, <see langword="false"/>.</para>
    /// \endif
    /// </returns>
    bool IsValidCoreIndex(int coreIndex);
}
