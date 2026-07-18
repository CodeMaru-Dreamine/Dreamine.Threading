using Dreamine.Threading.Models;

namespace Dreamine.Threading.Interfaces;

/// <summary>
/// \if KO
/// <para>Dreamine 작업자 스레드가 실행하는 작업 계약을 정의합니다.</para>
/// \endif
/// \if EN
/// <para>Defines a job contract executed by a Dreamine worker thread.</para>
/// \endif
/// </summary>
public interface IDreamineThreadJob
{
    /// <summary>
    /// \if KO
    /// <para>작업 이름을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the job name.</para>
    /// \endif
    /// </summary>
    string Name { get; }

    /// <summary>
    /// \if KO
    /// <para>정규화된 작업 옵션을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the normalized job options.</para>
    /// \endif
    /// </summary>
    DreamineThreadJobOptions Options { get; }

    /// <summary>
    /// \if KO
    /// <para>완료된 실행 횟수를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the number of completed executions.</para>
    /// \endif
    /// </summary>
    long ExecuteCount { get; }

    /// <summary>
    /// \if KO
    /// <para>마지막 실행 시각을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the most recent execution time.</para>
    /// \endif
    /// </summary>
    DateTimeOffset? LastExecutedAt { get; }

    /// <summary>
    /// \if KO
    /// <para>마지막 실행에서 발생한 예외를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the exception from the most recent execution.</para>
    /// \endif
    /// </summary>
    Exception? LastException { get; }

    /// <summary>
    /// \if KO
    /// <para>지정한 시각에 이 작업을 실행해야 하는지 확인합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Determines whether this job should run at the specified time.</para>
    /// \endif
    /// </summary>
    /// <param name="now">
    /// \if KO
    /// <para>실행 여부를 평가할 현재 시각입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The current time at which execution is evaluated.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>실행해야 하면 <see langword="true"/>, 아니면 <see langword="false"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when the job should run; otherwise, <see langword="false"/>.</para>
    /// \endif
    /// </returns>
    bool ShouldRun(DateTimeOffset now);

    /// <summary>
    /// \if KO
    /// <para>작업 대리자를 비동기로 실행하고 실행 통계를 갱신합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Executes the job delegate asynchronously and updates execution statistics.</para>
    /// \endif
    /// </summary>
    /// <param name="cancellationToken">
    /// \if KO
    /// <para>작업 실행을 취소하는 토큰입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A token that cancels job execution.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>비동기 작업 실행을 나타내는 값 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A value task representing asynchronous job execution.</para>
    /// \endif
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// \if KO
    /// <para>작업이 취소되면 구현체가 발생시킬 수 있습니다.</para>
    /// \endif
    /// \if EN
    /// <para>May be thrown by an implementation when execution is canceled.</para>
    /// \endif
    /// </exception>
    ValueTask ExecuteAsync(CancellationToken cancellationToken);
}
