using Dreamine.Threading.Models;

namespace Dreamine.Threading.Interfaces;

/// <summary>
/// Defines a manager that creates and controls Dreamine worker threads and polling jobs.
/// </summary>
public interface IDreamineThreadManager : IDisposable
{
    /// <summary>
    /// Registers a worker thread or an overflow polling job according to the specified options.
    /// </summary>
    /// <param name="options">The thread options.</param>
    /// <param name="action">The job action.</param>
    /// <returns>The registered thread job.</returns>
    IDreamineThreadJob Register(
        DreamineThreadOptions options,
        Func<CancellationToken, ValueTask> action);

    /// <summary>
    /// Starts the specified worker thread.
    /// </summary>
    /// <param name="threadName">The worker thread name.</param>
    /// <returns><see langword="true"/> if the worker thread was found and started; otherwise, <see langword="false"/>.</returns>
    bool Start(string threadName);

    /// <summary>
    /// Stops the specified worker thread.
    /// </summary>
    /// <param name="threadName">The worker thread name.</param>
    /// <returns><see langword="true"/> if the worker thread was found and stopped; otherwise, <see langword="false"/>.</returns>
    bool Stop(string threadName);

    /// <summary>
    /// Pauses the specified worker thread.
    /// </summary>
    /// <param name="threadName">The worker thread name.</param>
    /// <returns><see langword="true"/> if the worker thread was found and paused; otherwise, <see langword="false"/>.</returns>
    bool Pause(string threadName);

    /// <summary>
    /// Resumes the specified worker thread.
    /// </summary>
    /// <param name="threadName">The worker thread name.</param>
    /// <returns><see langword="true"/> if the worker thread was found and resumed; otherwise, <see langword="false"/>.</returns>
    bool Resume(string threadName);

    /// <summary>
    /// Starts all worker threads.
    /// </summary>
    void StartAll();

    /// <summary>
    /// Stops all worker threads.
    /// </summary>
    void StopAll();

    /// <summary>
    /// Pauses all worker threads.
    /// </summary>
    void PauseAll();

    /// <summary>
    /// Resumes all worker threads.
    /// </summary>
    void ResumeAll();

    /// <summary>
    /// Tries to get the specified worker thread.
    /// </summary>
    /// <param name="threadName">The worker thread name.</param>
    /// <param name="thread">The found worker thread.</param>
    /// <returns><see langword="true"/> if the worker thread was found; otherwise, <see langword="false"/>.</returns>
    bool TryGetThread(string threadName, out IDreamineThread? thread);

    /// <summary>
    /// Gets the registered worker threads.
    /// </summary>
    /// <returns>The registered worker threads.</returns>
    IReadOnlyList<IDreamineThread> GetThreads();

    /// <summary>
    /// Gets snapshots for all registered worker threads.
    /// </summary>
    /// <returns>The thread state snapshots.</returns>
    IReadOnlyList<DreamineThreadInfo> GetThreadInfos();
}