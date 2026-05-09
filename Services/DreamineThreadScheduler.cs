using Dreamine.Threading.Interfaces;

namespace Dreamine.Threading.Services;

/// <summary>
/// Provides default scheduling for overflow polling jobs.
/// </summary>
public sealed class DreamineThreadScheduler : IDreamineThreadScheduler
{
    /// <inheritdoc />
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