using System;

namespace Dreamine.Threading.Options;

/// <summary>
/// \if KO
/// <para>Dreamine 스레딩 서비스 등록 옵션을 제공합니다.</para>
/// \endif
/// \if EN
/// <para>Provides configuration options for Dreamine threading-service registration.</para>
/// \endif
/// </summary>
public sealed class DreamineThreadingOptions
{
    /// <summary>
    /// \if KO
    /// <para>Windows 전용 스레딩 서비스를 등록할지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether Windows-specific threading services are registered.</para>
    /// \endif
    /// </summary>
    public bool RegisterWindowsServices { get; set; } = true;

    /// <summary>
    /// \if KO
    /// <para>적응형 CPU 주기 정책을 사용할지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether the adaptive CPU cycle policy is used.</para>
    /// \endif
    /// </summary>
    public bool UseAdaptiveCpuPolicy { get; set; } = true;

    /// <summary>
    /// \if KO
    /// <para>기존 서비스 등록을 덮어쓸 수 있는지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether an existing service registration can be overwritten.</para>
    /// \endif
    /// </summary>
    public bool AllowOverride { get; set; } = true;
}
