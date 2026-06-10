namespace Dreamine.Threading.Interfaces;

/// <summary>
/// Provides CPU topology information for threading services.
/// </summary>
public interface ICpuInfoProvider
{
    /// <summary>
    /// Gets the logical processor count.
    /// </summary>
    /// <returns>The logical processor count.</returns>
    int GetLogicalProcessorCount();

    /// <summary>
    /// Determines whether the specified CPU core index is valid.
    /// </summary>
    /// <param name="coreIndex">The CPU core index.</param>
    /// <returns>True if the core index is valid; otherwise false.</returns>
    bool IsValidCoreIndex(int coreIndex);
}
