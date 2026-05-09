namespace Dreamine.Threading.Interfaces;

/// <summary>
/// Defines a platform service that applies CPU affinity to the current thread.
/// </summary>
public interface IThreadAffinityService
{
    /// <summary>
    /// Applies CPU affinity to the current thread.
    /// </summary>
    /// <param name="coreIndex">The CPU core index.</param>
    void ApplyToCurrentThread(int coreIndex);

    /// <summary>
    /// Clears CPU affinity from the current thread when supported.
    /// </summary>
    void ClearCurrentThreadAffinity();
}