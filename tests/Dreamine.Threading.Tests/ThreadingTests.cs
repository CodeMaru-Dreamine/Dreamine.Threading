using Dreamine.Threading.Allocators;
using Dreamine.Threading.Interfaces;
using Dreamine.Threading.Models;
using Dreamine.Threading.Policies;
using Dreamine.Threading.Services;
using Xunit;

namespace Dreamine.Threading.Tests;

public sealed class ThreadingTests
{
    [Fact]
    public void ThreadOptions_NormalizeInvalidValues()
    {
        var source = new DreamineThreadOptions
        {
            Name = " ",
            IntervalMs = -1,
            AutoThreadsPerCore = 0,
            OverflowPollingIntervalMs = -1,
            StopTimeout = TimeSpan.Zero,
            AutoStart = false
        };

        var result = source.Normalize();

        Assert.Equal("DreamineThread", result.Name);
        Assert.Equal(10, result.IntervalMs);
        Assert.Equal(2, result.AutoThreadsPerCore);
        Assert.Equal(100, result.OverflowPollingIntervalMs);
        Assert.Equal(TimeSpan.FromSeconds(2), result.StopTimeout);
        Assert.False(result.AutoStart);
    }

    [Fact]
    public void JobOptions_NormalizeAndPreserveFlags()
    {
        var result = new DreamineThreadJobOptions
        {
            Name = "",
            IntervalMs = -4,
            IsEnabled = false,
            IsOverflowPolling = true
        }.Normalize();

        Assert.Equal("DreamineThreadJob", result.Name);
        Assert.Equal(10, result.IntervalMs);
        Assert.False(result.IsEnabled);
        Assert.True(result.IsOverflowPolling);
    }

    [Fact]
    public async Task Job_TracksSuccessFailureAndSchedule()
    {
        var calls = 0;
        var job = new DreamineThreadJob(
            new DreamineThreadJobOptions { Name = "poll", IntervalMs = 100 },
            _ => { calls++; return ValueTask.CompletedTask; });

        Assert.True(job.ShouldRun(DateTimeOffset.UtcNow));
        await job.ExecuteAsync(default);
        Assert.Equal(1, calls);
        Assert.Equal(1, job.ExecuteCount);
        Assert.NotNull(job.LastExecutedAt);
        Assert.False(job.ShouldRun(job.LastExecutedAt!.Value.AddMilliseconds(99)));
        Assert.True(job.ShouldRun(job.LastExecutedAt.Value.AddMilliseconds(100)));

        var failure = new InvalidOperationException("boom");
        var failing = new DreamineThreadJob(new(), _ => ValueTask.FromException(failure));
        Assert.Same(failure, await Assert.ThrowsAsync<InvalidOperationException>(
            () => failing.ExecuteAsync(default).AsTask()));
        Assert.Same(failure, failing.LastException);
        Assert.Equal(0, failing.ExecuteCount);
    }

    [Fact]
    public void Job_DisabledNeverRunsAndZeroIntervalAlwaysRuns()
    {
        var disabled = new DreamineThreadJob(
            new DreamineThreadJobOptions { IsEnabled = false },
            _ => ValueTask.CompletedTask);
        var immediate = new DreamineThreadJob(
            new DreamineThreadJobOptions { IntervalMs = 0 },
            _ => ValueTask.CompletedTask);

        Assert.False(disabled.ShouldRun(DateTimeOffset.UtcNow));
        Assert.True(immediate.ShouldRun(DateTimeOffset.UtcNow));
    }

    [Theory]
    [InlineData(20, 0)]
    [InlineData(30, 1)]
    [InlineData(50, 3)]
    [InlineData(70, 5)]
    public void AdaptivePolicy_UsesCpuThresholds(double cpu, int expected)
    {
        var policy = new AdaptiveCpuCyclePolicy(new CpuUsage(cpu));
        var delay = policy.GetDelayMs(
            new DreamineThreadOptions { IntervalMs = 0 },
            DreamineThreadCoreAssignment.None(),
            Context());

        Assert.Equal(expected, delay);
    }

    [Fact]
    public void CyclePolicies_HandleFixedAndOverflowIntervals()
    {
        var options = new DreamineThreadOptions { IntervalMs = 25, OverflowPollingIntervalMs = 77 };
        var normal = DreamineThreadCoreAssignment.None();
        var overflow = DreamineThreadCoreAssignment.Overflow();
        var context = Context();

        Assert.Equal(25, new FixedIntervalCyclePolicy().GetDelayMs(options, normal, context));
        Assert.Equal(77, new FixedIntervalCyclePolicy().GetDelayMs(options, overflow, context));
        Assert.Equal(77, new OverflowPollingPolicy().GetDelayMs(options, normal, context));
    }

    [Fact]
    public void Allocator_AssignsLeastUsedCoreAndOverflowsAtCapacity()
    {
        var allocator = new AutoCoreAllocator(2);
        var options = new DreamineThreadOptions
        {
            CoreMode = DreamineThreadCoreMode.Auto,
            AutoThreadsPerCore = 1
        };

        var first = allocator.Allocate(options);
        var second = allocator.Allocate(options);
        var overflow = allocator.Allocate(options);

        Assert.Equal(0, first.CoreIndex);
        Assert.Equal(1, second.CoreIndex);
        Assert.True(overflow.IsOverflowPolling);

        allocator.Release(first);
        Assert.Equal(0, allocator.Allocate(options).CoreIndex);
    }

    [Fact]
    public void Allocator_HandlesNoneManualAndInvalidManualCore()
    {
        var allocator = new AutoCoreAllocator(4);

        Assert.Null(allocator.Allocate(new() { CoreMode = DreamineThreadCoreMode.None }).CoreIndex);
        Assert.Equal(2, allocator.Allocate(new()
        {
            CoreMode = DreamineThreadCoreMode.Manual,
            CoreIndex = 2
        }).CoreIndex);
        Assert.Throws<ArgumentOutOfRangeException>(() => allocator.Allocate(new()
        {
            CoreMode = DreamineThreadCoreMode.Manual,
            CoreIndex = 9
        }));
    }

    private sealed class CpuUsage(double value) : ICpuUsageProvider
    {
        public double GetTotalCpuUsagePercent() => value;
    }

    private static DreamineThreadCycleContext Context() =>
        new("worker", 1, 1, null, false, DateTimeOffset.UtcNow);
}
