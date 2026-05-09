namespace Dreamine.Threading.Interfaces;

/// <summary>
/// Defines a service that provides CPU usage information.
/// </summary>
public interface ICpuUsageProvider
{
    /// <summary>
    /// Gets the total CPU usage percentage.
    /// </summary>
    /// <returns>The total CPU usage percentage.</returns>
    double GetTotalCpuUsagePercent();
}