using Dreamine.Threading.Models;

namespace Dreamine.Threading.Interfaces;

/// <summary>
/// Defines a service that assigns worker threads to CPU cores.
/// </summary>
public interface IThreadCoreAllocator
{
    /// <summary>
    /// Allocates a CPU core assignment for the specified thread options.
    /// </summary>
    /// <param name="options">The thread options.</param>
    /// <returns>The CPU core assignment.</returns>
    DreamineThreadCoreAssignment Allocate(DreamineThreadOptions options);

    /// <summary>
    /// Releases a previously allocated CPU core assignment.
    /// </summary>
    /// <param name="assignment">The CPU core assignment to release.</param>
    void Release(DreamineThreadCoreAssignment assignment);
}