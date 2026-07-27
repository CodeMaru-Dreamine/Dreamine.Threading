namespace Dreamine.Threading.Models;

/// <summary>
/// \if KO
/// <para>Dreamine 스레드 작업 생성 옵션을 나타냅니다.</para>
/// \endif
/// \if EN
/// <para>Represents options used to create a Dreamine thread job.</para>
/// \endif
/// </summary>
public sealed class DreamineThreadJobOptions
{
    /// <summary>
    /// \if KO
    /// <para>작업 이름을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the job name.</para>
    /// \endif
    /// </summary>
    public string Name { get; set; } = "DreamineThreadJob";

    /// <summary>
    /// \if KO
    /// <para>밀리초 단위 작업 실행 간격을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the job execution interval in milliseconds.</para>
    /// \endif
    /// </summary>
    public int IntervalMs { get; set; } = 10;

    /// <summary>
    /// \if KO
    /// <para>작업 활성화 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether the job is enabled.</para>
    /// \endif
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// \if KO
    /// <para>작업이 오버플로 폴링 작업인지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether the job is an overflow polling job.</para>
    /// \endif
    /// </summary>
    public bool IsOverflowPolling { get; set; }

    /// <summary>
    /// \if KO
    /// <para>잘못된 이름과 음수 간격을 기본값으로 보정한 복사본을 생성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates a copy with invalid names and negative intervals normalized to defaults.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>정규화된 새 작업 옵션입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A new normalized job-options instance.</para>
    /// \endif
    /// </returns>
    public DreamineThreadJobOptions Normalize()
    {
        return new DreamineThreadJobOptions
        {
            Name = string.IsNullOrWhiteSpace(Name) ? "DreamineThreadJob" : Name,
            IntervalMs = IntervalMs < 0 ? 10 : IntervalMs,
            IsEnabled = IsEnabled,
            IsOverflowPolling = IsOverflowPolling
        };
    }
}
