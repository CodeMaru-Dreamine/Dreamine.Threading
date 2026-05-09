namespace Dreamine.Threading.Interfaces;

/// <summary>
/// Defines a platform service that controls timer resolution.
/// </summary>
public interface ITimerResolutionService
{
    /// <summary>
    /// Begins high precision timer resolution.
    /// </summary>
    void Begin();

    /// <summary>
    /// Ends high precision timer resolution.
    /// </summary>
    void End();
}