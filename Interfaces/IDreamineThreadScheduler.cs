namespace Dreamine.Threading.Interfaces;

/// <summary>
/// Defines a scheduler that assigns polling jobs to worker threads.
/// </summary>
public interface IDreamineThreadScheduler
{
    /// <summary>
    /// Selects a worker thread for an overflow polling job.
    /// </summary>
    /// <param name="threads">The available worker threads.</param>
    /// <returns>The selected worker thread.</returns>
    IDreamineThread? SelectWorker(IReadOnlyList<IDreamineThread> threads);
}