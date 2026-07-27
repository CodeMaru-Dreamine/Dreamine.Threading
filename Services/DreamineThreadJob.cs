using Dreamine.Threading.Interfaces;
using Dreamine.Threading.Models;

namespace Dreamine.Threading.Services;

/// <summary>
/// \if KO
/// <para>기본 Dreamine 스레드 작업 구현을 제공합니다.</para>
/// \endif
/// \if EN
/// <para>Provides the default Dreamine thread-job implementation.</para>
/// \endif
/// </summary>
public sealed class DreamineThreadJob : IDreamineThreadJob
{
    /// <summary>
    /// \if KO
    /// <para>action 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the action value.</para>
    /// \endif
    /// </summary>
    private readonly Func<CancellationToken, ValueTask> _action;
    /// <summary>
    /// \if KO
    /// <para>last Executed At 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the last executed at value.</para>
    /// \endif
    /// </summary>
    private DateTimeOffset? _lastExecutedAt;
    /// <summary>
    /// \if KO
    /// <para>execute Count 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the execute count value.</para>
    /// \endif
    /// </summary>
    private long _executeCount;

    /// <summary>
    /// \if KO
    /// <para>정규화된 작업 이름을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the normalized job name.</para>
    /// \endif
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// \if KO
    /// <para>정규화된 작업 옵션을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the normalized job options.</para>
    /// \endif
    /// </summary>
    public DreamineThreadJobOptions Options { get; }

    /// <summary>
    /// \if KO
    /// <para>성공적으로 완료된 실행 횟수를 원자적으로 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Atomically gets the number of successfully completed executions.</para>
    /// \endif
    /// </summary>
    public long ExecuteCount => Interlocked.Read(ref _executeCount);

    /// <summary>
    /// \if KO
    /// <para>마지막 성공 실행 시각을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the most recent successful execution time.</para>
    /// \endif
    /// </summary>
    public DateTimeOffset? LastExecutedAt => _lastExecutedAt;

    /// <summary>
    /// \if KO
    /// <para>마지막 실패에서 발생한 예외를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the exception raised by the most recent failed execution.</para>
    /// \endif
    /// </summary>
    public Exception? LastException { get; private set; }

    /// <summary>
    /// \if KO
    /// <para>지정한 옵션과 비동기 대리자로 <see cref="T:Dreamine.Threading.Services.DreamineThreadJob" /> 클래스의 새 인스턴스를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes a new instance of <see cref="T:Dreamine.Threading.Services.DreamineThreadJob" /> with the specified options and asynchronous delegate.</para>
    /// \endif
    /// </summary>
    /// <param name="options">
    /// \if KO
    /// <para>정규화할 작업 옵션입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The job options to normalize.</para>
    /// \endif
    /// </param>
    /// <param name="action">
    /// \if KO
    /// <para>작업 실행 시 호출할 비동기 대리자입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The asynchronous delegate invoked when the job executes.</para>
    /// \endif
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// \if KO
    /// <para><paramref name="options"/> 또는 <paramref name="action"/>이 <see langword="null"/>일 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when <paramref name="options"/> or <paramref name="action"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
    public DreamineThreadJob(
        DreamineThreadJobOptions options,
        Func<CancellationToken, ValueTask> action)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(action);

        Options = options.Normalize();
        Name = Options.Name;
        _action = action;
    }

    /// <summary>
    /// \if KO
    /// <para>활성 상태와 마지막 성공 실행 시각을 기준으로 현재 작업 실행 여부를 판단합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Determines whether the job should run from its enabled state and last successful execution time.</para>
    /// \endif
    /// </summary>
    /// <param name="now">
    /// \if KO
    /// <para>실행 간격을 평가할 현재 시각입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The current time used to evaluate the execution interval.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>활성화되어 있고 최초 실행이거나 간격이 경과했으면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when enabled and either never executed or its interval has elapsed.</para>
    /// \endif
    /// </returns>
    public bool ShouldRun(DateTimeOffset now)
    {
        if (!Options.IsEnabled)
        {
            return false;
        }

        if (Options.IntervalMs == 0)
        {
            return true;
        }

        if (_lastExecutedAt is null)
        {
            return true;
        }

        var elapsedMs = (now - _lastExecutedAt.Value).TotalMilliseconds;
        return elapsedMs >= Options.IntervalMs;
    }

    /// <summary>
    /// \if KO
    /// <para>작업 대리자를 실행하고 성공 통계 또는 마지막 예외를 갱신합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Executes the job delegate and updates success statistics or the last exception.</para>
    /// \endif
    /// </summary>
    /// <param name="cancellationToken">
    /// \if KO
    /// <para>작업 대리자에 전달할 취소 토큰입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The cancellation token passed to the job delegate.</para>
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
    /// <para>대리자가 취소를 보고하면 그대로 전파됩니다.</para>
    /// \endif
    /// \if EN
    /// <para>Propagated unchanged when the delegate reports cancellation.</para>
    /// \endif
    /// </exception>
    /// <exception cref="Exception">
    /// \if KO
    /// <para>대리자가 발생시킨 비취소 예외를 <see cref="LastException"/>에 저장한 뒤 다시 전파합니다.</para>
    /// \endif
    /// \if EN
    /// <para>A non-cancellation exception raised by the delegate is stored in <see cref="LastException"/> and rethrown.</para>
    /// \endif
    /// </exception>
    public async ValueTask ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _action(cancellationToken).ConfigureAwait(false);

            _lastExecutedAt = DateTimeOffset.Now;
            Interlocked.Increment(ref _executeCount);
            LastException = null;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            LastException = ex;
            throw;
        }
    }
}
