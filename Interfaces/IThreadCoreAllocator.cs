using Dreamine.Threading.Models;

namespace Dreamine.Threading.Interfaces;

/// <summary>
/// \if KO
/// <para>작업자 스레드를 CPU 코어에 할당하는 서비스 계약을 정의합니다.</para>
/// \endif
/// \if EN
/// <para>Defines a service contract that assigns worker threads to CPU cores.</para>
/// \endif
/// </summary>
public interface IThreadCoreAllocator
{
    /// <summary>
    /// \if KO
    /// <para>지정한 스레드 옵션에 맞는 CPU 코어 할당을 만듭니다.</para>
    /// \endif
    /// \if EN
    /// <para>Allocates a CPU core assignment for the specified thread options.</para>
    /// \endif
    /// </summary>
    /// <param name="options">
    /// \if KO
    /// <para>코어 모드와 수동 코어를 포함하는 스레드 옵션입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The thread options containing core mode and manual-core selection.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>선택된 CPU 코어 할당입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The selected CPU core assignment.</para>
    /// \endif
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// \if KO
    /// <para><paramref name="options"/>가 <see langword="null"/>이면 구현체가 발생시킬 수 있습니다.</para>
    /// \endif
    /// \if EN
    /// <para>May be thrown by an implementation when <paramref name="options"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
    DreamineThreadCoreAssignment Allocate(DreamineThreadOptions options);

    /// <summary>
    /// \if KO
    /// <para>이전에 할당한 CPU 코어 할당을 해제합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Releases a previously allocated CPU core assignment.</para>
    /// \endif
    /// </summary>
    /// <param name="assignment">
    /// \if KO
    /// <para>해제할 CPU 코어 할당입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The CPU core assignment to release.</para>
    /// \endif
    /// </param>
    void Release(DreamineThreadCoreAssignment assignment);
}
