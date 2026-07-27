using Dreamine.Threading.Interfaces;

namespace Dreamine.Threading.Services;

/// <summary>
/// \if KO
/// <para>오버플로 폴링 작업을 위한 기본 스케줄링을 제공합니다.</para>
/// \endif
/// \if EN
/// <para>Provides default scheduling for overflow polling jobs.</para>
/// \endif
/// </summary>
public sealed class DreamineThreadScheduler : IDreamineThreadScheduler
{
    /// <summary>
    /// \if KO
    /// <para>작업 수가 가장 적고 이름 순서가 빠른 작업자 스레드를 선택합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Selects the worker thread with the fewest jobs, breaking ties by name.</para>
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
    /// <para>선택된 작업자이며 목록이 비어 있으면 <see langword="null"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The selected worker, or <see langword="null"/> when the list is empty.</para>
    /// \endif
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// \if KO
    /// <para><paramref name="threads"/>가 <see langword="null"/>일 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when <paramref name="threads"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
    public IDreamineThread? SelectWorker(IReadOnlyList<IDreamineThread> threads)
    {
        ArgumentNullException.ThrowIfNull(threads);

        if (threads.Count == 0)
        {
            return null;
        }

        return threads
            .OrderBy(thread => thread.JobCount)
            .ThenBy(thread => thread.Name, StringComparer.Ordinal)
            .FirstOrDefault();
    }
}
