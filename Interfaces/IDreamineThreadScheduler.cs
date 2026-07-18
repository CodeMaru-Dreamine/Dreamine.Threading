namespace Dreamine.Threading.Interfaces;

/// <summary>
/// \if KO
/// <para>폴링 작업을 작업자 스레드에 할당하는 스케줄러 계약을 정의합니다.</para>
/// \endif
/// \if EN
/// <para>Defines a scheduler contract that assigns polling jobs to worker threads.</para>
/// \endif
/// </summary>
public interface IDreamineThreadScheduler
{
    /// <summary>
    /// \if KO
    /// <para>오버플로 폴링 작업을 실행할 작업자 스레드를 선택합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Selects a worker thread for an overflow polling job.</para>
    /// \endif
    /// </summary>
    /// <param name="threads">
    /// \if KO
    /// <para>선택 가능한 작업자 스레드 목록입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The available worker threads from which to select.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>선택된 작업자 스레드이며 적합한 스레드가 없으면 <see langword="null"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The selected worker thread, or <see langword="null"/> when none is suitable.</para>
    /// \endif
    /// </returns>
    IDreamineThread? SelectWorker(IReadOnlyList<IDreamineThread> threads);
}
