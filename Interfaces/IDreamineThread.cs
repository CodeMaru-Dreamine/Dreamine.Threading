using Dreamine.Threading.Models;

namespace Dreamine.Threading.Interfaces;

/// <summary>
/// \if KO
/// <para>Dreamine 작업자 스레드의 제어 및 진단 계약을 정의합니다.</para>
/// \endif
/// \if EN
/// <para>Defines the control and diagnostic contract for a Dreamine worker thread.</para>
/// \endif
/// </summary>
public interface IDreamineThread : IDisposable
{
    /// <summary>
    /// \if KO
    /// <para>작업자 스레드 이름을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the worker-thread name.</para>
    /// \endif
    /// </summary>
    string Name { get; }

    /// <summary>
    /// \if KO
    /// <para>정규화된 작업자 스레드 옵션을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the normalized worker-thread options.</para>
    /// \endif
    /// </summary>
    DreamineThreadOptions Options { get; }

    /// <summary>
    /// \if KO
    /// <para>CPU 코어 할당을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the CPU core assignment.</para>
    /// \endif
    /// </summary>
    DreamineThreadCoreAssignment CoreAssignment { get; }

    /// <summary>
    /// \if KO
    /// <para>현재 작업자 스레드 상태를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the current worker-thread status.</para>
    /// \endif
    /// </summary>
    DreamineThreadStatus Status { get; }

    /// <summary>
    /// \if KO
    /// <para>이 작업자 스레드에 할당된 작업 수를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the number of jobs assigned to this worker thread.</para>
    /// \endif
    /// </summary>
    int JobCount { get; }

    /// <summary>
    /// \if KO
    /// <para>이 작업자 스레드에 실행할 작업을 추가합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Adds an executable job to this worker thread.</para>
    /// \endif
    /// </summary>
    /// <param name="job">
    /// \if KO
    /// <para>추가할 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The job to add.</para>
    /// \endif
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// \if KO
    /// <para><paramref name="job"/>이 <see langword="null"/>이면 구현체가 발생시킬 수 있습니다.</para>
    /// \endif
    /// \if EN
    /// <para>May be thrown by an implementation when <paramref name="job"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// \if KO
    /// <para>스레드가 이미 정리되었으면 구현체가 발생시킬 수 있습니다.</para>
    /// \endif
    /// \if EN
    /// <para>May be thrown by an implementation when the thread has been disposed.</para>
    /// \endif
    /// </exception>
    void AddJob(IDreamineThreadJob job);

    /// <summary>
    /// \if KO
    /// <para>작업자 스레드를 시작합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Starts the worker thread.</para>
    /// \endif
    /// </summary>
    void Start();

    /// <summary>
    /// \if KO
    /// <para>작업자 스레드 실행을 일시 정지합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Pauses worker-thread execution.</para>
    /// \endif
    /// </summary>
    void Pause();

    /// <summary>
    /// \if KO
    /// <para>일시 정지된 작업자 스레드를 재개합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Resumes a paused worker thread.</para>
    /// \endif
    /// </summary>
    void Resume();

    /// <summary>
    /// \if KO
    /// <para>작업자 스레드를 중지하고 종료 대기 시간 동안 호출자를 차단합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stops the worker thread and blocks the caller during the join wait.</para>
    /// \endif
    /// </summary>
    void Stop();

    /// <summary>
    /// \if KO
    /// <para>종료 대기 시간 동안 호출 스레드를 차단하지 않고 작업자 스레드를 중지합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stops the worker thread without blocking the caller thread during the join wait.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>비동기 중지 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The asynchronous stop operation.</para>
    /// \endif
    /// </returns>
    ValueTask StopAsync();

    /// <summary>
    /// \if KO
    /// <para>작업자 스레드 상태의 스냅샷을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets a snapshot of the worker-thread state.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>현재 스레드 상태 스냅샷입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The current thread-state snapshot.</para>
    /// \endif
    /// </returns>
    DreamineThreadInfo GetInfo();
}
