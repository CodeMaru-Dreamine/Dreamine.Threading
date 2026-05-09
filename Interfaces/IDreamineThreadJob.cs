using Dreamine.Threading.Models;

namespace Dreamine.Threading.Interfaces;

/// <summary>
/// Defines a job executed by a Dreamine worker thread.
/// </summary>
public interface IDreamineThreadJob
{
    /// <summary>
    /// Gets the job name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the job options.
    /// </summary>
    DreamineThreadJobOptions Options { get; }

    /// <summary>
    /// Gets the number of executions.
    /// </summary>
    long ExecuteCount { get; }

    /// <summary>
    /// Gets the last executed time.
    /// </summary>
    DateTimeOffset? LastExecutedAt { get; }

    /// <summary>
    /// Gets the last exception.
    /// </summary>
    Exception? LastException { get; }

    /// <summary>
    /// Determines whether this job should run at the specified time.
    /// </summary>
    /// <param name="now">The current time.</param>
    /// <returns>True if the job should run; otherwise false.</returns>
    bool ShouldRun(DateTimeOffset now);

    /// <summary>
    /// Executes the job.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The asynchronous operation.</returns>
    ValueTask ExecuteAsync(CancellationToken cancellationToken);
}