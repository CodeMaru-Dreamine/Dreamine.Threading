namespace Dreamine.Threading.Models;

/// <summary>
/// \if KO
/// <para>Dreamine 스레드의 수명 주기 상태를 정의합니다.</para>
/// \endif
/// \if EN
/// <para>Defines the lifecycle status of a Dreamine thread.</para>
/// \endif
/// </summary>
public enum DreamineThreadStatus
{
    /// <summary>
    /// \if KO
    /// <para>스레드가 생성되었지만 시작되지 않았습니다.</para>
    /// \endif
    /// \if EN
    /// <para>The thread has been created but not started.</para>
    /// \endif
    /// </summary>
    Created = 0,

    /// <summary>
    /// \if KO
    /// <para>스레드가 실행 중입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The thread is running.</para>
    /// \endif
    /// </summary>
    Running = 1,

    /// <summary>
    /// \if KO
    /// <para>스레드가 일시 정지되었습니다.</para>
    /// \endif
    /// \if EN
    /// <para>The thread is paused.</para>
    /// \endif
    /// </summary>
    Paused = 2,

    /// <summary>
    /// \if KO
    /// <para>스레드가 중지되는 중입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The thread is stopping.</para>
    /// \endif
    /// </summary>
    Stopping = 3,

    /// <summary>
    /// \if KO
    /// <para>스레드가 중지되었습니다.</para>
    /// \endif
    /// \if EN
    /// <para>The thread has stopped.</para>
    /// \endif
    /// </summary>
    Stopped = 4,

    /// <summary>
    /// \if KO
    /// <para>예외로 인해 스레드가 실패했습니다.</para>
    /// \endif
    /// \if EN
    /// <para>The thread has failed because of an exception.</para>
    /// \endif
    /// </summary>
    Faulted = 5,

    /// <summary>
    /// \if KO
    /// <para>스레드가 정리되었습니다.</para>
    /// \endif
    /// \if EN
    /// <para>The thread has been disposed.</para>
    /// \endif
    /// </summary>
    Disposed = 6
}
