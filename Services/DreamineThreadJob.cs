using Dreamine.Threading.Interfaces;
using Dreamine.Threading.Models;

namespace Dreamine.Threading.Services;

/// <summary>
/// Provides the default Dreamine thread job implementation.
/// </summary>
public sealed class DreamineThreadJob : IDreamineThreadJob
{
    private readonly Func<CancellationToken, ValueTask> _action;
    private DateTimeOffset? _lastExecutedAt;
    private long _executeCount;

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public DreamineThreadJobOptions Options { get; }

    /// <inheritdoc />
    public long ExecuteCount => Interlocked.Read(ref _executeCount);

    /// <inheritdoc />
    public DateTimeOffset? LastExecutedAt => _lastExecutedAt;

    /// <inheritdoc />
    public Exception? LastException { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DreamineThreadJob"/> class.
    /// </summary>
    /// <param name="options">The job options.</param>
    /// <param name="action">The job action.</param>
    public DreamineThreadJob(
        DreamineThreadJobOptions options,
        Func<CancellationToken, ValueTask> action)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(action);

        Options = options.Normalize();
        Name = Options.Name;
        _action = action;
    }

    /// <inheritdoc />
    public bool ShouldRun(DateTimeOffset now)
    {
        if (!Options.IsEnabled)
        {
            return false;
        }

        if (Options.IntervalMs == 0)
        {
            return true;
        }

        if (_lastExecutedAt is null)
        {
            return true;
        }

        var elapsedMs = (now - _lastExecutedAt.Value).TotalMilliseconds;
        return elapsedMs >= Options.IntervalMs;
    }

    /// <inheritdoc />
    public async ValueTask ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _action(cancellationToken).ConfigureAwait(false);

            _lastExecutedAt = DateTimeOffset.Now;
            Interlocked.Increment(ref _executeCount);
            LastException = null;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            LastException = ex;
            throw;
        }
    }
}