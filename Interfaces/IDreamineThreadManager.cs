using Dreamine.Threading.Models;

namespace Dreamine.Threading.Interfaces;

/// <summary>
/// \if KO
/// <para>Dreamine 작업자 스레드와 폴링 작업을 생성하고 제어하는 관리자 계약을 정의합니다.</para>
/// \endif
/// \if EN
/// <para>Defines a manager contract that creates and controls Dreamine worker threads and polling jobs.</para>
/// \endif
/// </summary>
public interface IDreamineThreadManager : IDisposable
{
    /// <summary>
    /// \if KO
    /// <para>지정한 옵션에 따라 작업자 스레드 또는 오버플로 폴링 작업을 등록합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Registers a worker thread or overflow polling job according to the specified options.</para>
    /// \endif
    /// </summary>
    /// <param name="options">
    /// \if KO
    /// <para>스레드, 코어 및 주기 설정입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The thread, core, and cycle settings.</para>
    /// \endif
    /// </param>
    /// <param name="action">
    /// \if KO
    /// <para>각 실행 시 호출할 비동기 작업 대리자입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The asynchronous job delegate invoked on each execution.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>등록된 스레드 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The registered thread job.</para>
    /// \endif
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// \if KO
    /// <para>인수 중 하나가 <see langword="null"/>이면 구현체가 발생시킬 수 있습니다.</para>
    /// \endif
    /// \if EN
    /// <para>May be thrown by an implementation when an argument is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// \if KO
    /// <para>관리자가 이미 정리되었으면 구현체가 발생시킬 수 있습니다.</para>
    /// \endif
    /// \if EN
    /// <para>May be thrown by an implementation when the manager has been disposed.</para>
    /// \endif
    /// </exception>
    IDreamineThreadJob Register(
        DreamineThreadOptions options,
        Func<CancellationToken, ValueTask> action);

    /// <summary>
    /// \if KO
    /// <para>지정한 작업자 스레드를 시작합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Starts the specified worker thread.</para>
    /// \endif
    /// </summary>
    /// <param name="threadName">
    /// \if KO
    /// <para>시작할 작업자 스레드 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The name of the worker thread to start.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>찾아서 시작했으면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when the worker was found and started.</para>
    /// \endif
    /// </returns>
    bool Start(string threadName);

    /// <summary>
    /// \if KO
    /// <para>지정한 작업자 스레드를 동기적으로 중지합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stops the specified worker thread synchronously.</para>
    /// \endif
    /// </summary>
    /// <param name="threadName">
    /// \if KO
    /// <para>중지할 작업자 스레드 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The name of the worker thread to stop.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>찾아서 중지했으면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when the worker was found and stopped.</para>
    /// \endif
    /// </returns>
    bool Stop(string threadName);

    /// <summary>
    /// \if KO
    /// <para>종료 대기 동안 호출 스레드를 차단하지 않고 지정한 작업자 스레드를 중지합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stops the specified worker thread without blocking the caller during the join wait.</para>
    /// \endif
    /// </summary>
    /// <param name="threadName">
    /// \if KO
    /// <para>중지할 작업자 스레드 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The name of the worker thread to stop.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>찾아서 중지했는지를 나타내는 비동기 결과입니다.</para>
    /// \endif
    /// \if EN
    /// <para>An asynchronous result indicating whether the worker was found and stopped.</para>
    /// \endif
    /// </returns>
    ValueTask<bool> StopAsync(string threadName);

    /// <summary>
    /// \if KO
    /// <para>지정한 작업자 스레드를 일시 정지합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Pauses the specified worker thread.</para>
    /// \endif
    /// </summary>
    /// <param name="threadName">
    /// \if KO
    /// <para>일시 정지할 스레드 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The name of the worker thread to pause.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>찾아서 일시 정지했으면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when the worker was found and paused.</para>
    /// \endif
    /// </returns>
    bool Pause(string threadName);

    /// <summary>
    /// \if KO
    /// <para>지정한 작업자 스레드를 재개합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Resumes the specified worker thread.</para>
    /// \endif
    /// </summary>
    /// <param name="threadName">
    /// \if KO
    /// <para>재개할 스레드 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The name of the worker thread to resume.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>찾아서 재개했으면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when the worker was found and resumed.</para>
    /// \endif
    /// </returns>
    bool Resume(string threadName);

    /// <summary>
    /// \if KO
    /// <para>등록된 모든 작업자 스레드를 시작합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Starts all registered worker threads.</para>
    /// \endif
    /// </summary>
    void StartAll();

    /// <summary>
    /// \if KO
    /// <para>등록된 모든 작업자 스레드를 동기적으로 중지합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stops all registered worker threads synchronously.</para>
    /// \endif
    /// </summary>
    void StopAll();

    /// <summary>
    /// \if KO
    /// <para>종료 대기 동안 호출 스레드를 차단하지 않고 모든 작업자 스레드를 중지합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stops all worker threads without blocking the caller during join waits.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>모든 스레드의 비동기 중지 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The asynchronous stop operation for all threads.</para>
    /// \endif
    /// </returns>
    ValueTask StopAllAsync();

    /// <summary>
    /// \if KO
    /// <para>등록된 모든 작업자 스레드를 일시 정지합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Pauses all registered worker threads.</para>
    /// \endif
    /// </summary>
    void PauseAll();

    /// <summary>
    /// \if KO
    /// <para>등록된 모든 작업자 스레드를 재개합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Resumes all registered worker threads.</para>
    /// \endif
    /// </summary>
    void ResumeAll();

    /// <summary>
    /// \if KO
    /// <para>이름으로 등록된 작업자 스레드를 가져오려고 시도합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Attempts to get a registered worker thread by name.</para>
    /// \endif
    /// </summary>
    /// <param name="threadName">
    /// \if KO
    /// <para>찾을 작업자 스레드 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The worker-thread name to find.</para>
    /// \endif
    /// </param>
    /// <param name="thread">
    /// \if KO
    /// <para>성공 시 찾은 작업자 스레드를 받습니다.</para>
    /// \endif
    /// \if EN
    /// <para>Receives the found worker thread on success.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>스레드를 찾았으면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when the worker was found.</para>
    /// \endif
    /// </returns>
    bool TryGetThread(string threadName, out IDreamineThread? thread);

    /// <summary>
    /// \if KO
    /// <para>등록된 작업자 스레드의 스냅샷 목록을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets a snapshot list of registered worker threads.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>등록된 작업자 스레드 목록입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The registered worker-thread list.</para>
    /// \endif
    /// </returns>
    IReadOnlyList<IDreamineThread> GetThreads();

    /// <summary>
    /// \if KO
    /// <para>모든 등록된 작업자 스레드의 상태 스냅샷을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets state snapshots for all registered worker threads.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>스레드 상태 스냅샷 목록입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The thread-state snapshot list.</para>
    /// \endif
    /// </returns>
    IReadOnlyList<DreamineThreadInfo> GetThreadInfos();
}
