namespace Dreamine.Threading.Models;

/// <summary>
/// Defines the lifecycle status of a Dreamine thread.
/// </summary>
public enum DreamineThreadStatus
{
    /// <summary>
    /// The thread has been created but not started.
    /// </summary>
    Created = 0,

    /// <summary>
    /// The thread is running.
    /// </summary>
    Running = 1,

    /// <summary>
    /// The thread is paused.
    /// </summary>
    Paused = 2,

    /// <summary>
    /// The thread is stopping.
    /// </summary>
    Stopping = 3,

    /// <summary>
    /// The thread has stopped.
    /// </summary>
    Stopped = 4,

    /// <summary>
    /// The thread has failed due to an exception.
    /// </summary>
    Faulted = 5,

    /// <summary>
    /// The thread has been disposed.
    /// </summary>
    Disposed = 6
}