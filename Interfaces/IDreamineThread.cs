using Dreamine.Threading.Models;

namespace Dreamine.Threading.Interfaces;

/// <summary>
/// Defines a Dreamine worker thread.
/// </summary>
public interface IDreamineThread : IDisposable
{
    /// <summary>
    /// Gets the worker thread name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the worker thread options.
    /// </summary>
    DreamineThreadOptions Options { get; }

    /// <summary>
    /// Gets the CPU core assignment.
    /// </summary>
    DreamineThreadCoreAssignment CoreAssignment { get; }

    /// <summary>
    /// Gets the current worker thread status.
    /// </summary>
    DreamineThreadStatus Status { get; }

    /// <summary>
    /// Gets the number of jobs assigned to this worker thread.
    /// </summary>
    int JobCount { get; }

    /// <summary>
    /// Adds a job to this worker thread.
    /// </summary>
    /// <param name="job">The job to add.</param>
    void AddJob(IDreamineThreadJob job);

    /// <summary>
    /// Starts the worker thread.
    /// </summary>
    void Start();

    /// <summary>
    /// Pauses the worker thread.
    /// </summary>
    void Pause();

    /// <summary>
    /// Resumes the worker thread.
    /// </summary>
    void Resume();

    /// <summary>
    /// Stops the worker thread.
    /// </summary>
    void Stop();

    /// <summary>
    /// Gets a snapshot of the worker thread state.
    /// </summary>
    /// <returns>The thread state snapshot.</returns>
    DreamineThreadInfo GetInfo();
}