using System;
using Dreamine.Logging.Interfaces;
using Dreamine.MVVM.Core;
using Dreamine.Threading.Allocators;
using Dreamine.Threading.Interfaces;
using Dreamine.Threading.Options;
using Dreamine.Threading.Policies;
using Dreamine.Threading.Services;

namespace Dreamine.Threading.Registration;

/// <summary>
/// Provides registration helpers for Dreamine threading core services.
/// </summary>
public static class DreamineThreadingRegistration
{
    /// <summary>
    /// Registers Dreamine threading core services.
    /// </summary>
    /// <param name="configure">The optional threading configuration action.</param>
    public static void Register(Action<DreamineThreadingOptions>? configure = null)
    {
        var options = new DreamineThreadingOptions();
        configure?.Invoke(options);

        ICpuUsageProvider? cpuUsageProvider = DMContainer.IsRegistered<ICpuUsageProvider>()
            ? DMContainer.Resolve<ICpuUsageProvider>()
            : null;

        Register(options, cpuUsageProvider);
    }

    /// <summary>
    /// Registers Dreamine threading core services.
    /// </summary>
    /// <param name="options">The threading options.</param>
    /// <param name="cpuUsageProvider">The optional CPU usage provider for adaptive cycle policy.</param>
    public static void Register(
        DreamineThreadingOptions options,
        ICpuUsageProvider? cpuUsageProvider = null)
    {
        ArgumentNullException.ThrowIfNull(options);

        DMContainer.RegisterSingleton<IThreadCoreAllocator>(
            new AutoCoreAllocator());

        DMContainer.RegisterSingleton<IThreadCyclePolicy>(
            options.UseAdaptiveCpuPolicy && cpuUsageProvider is not null
                ? new AdaptiveCpuCyclePolicy(cpuUsageProvider)
                : new FixedIntervalCyclePolicy());

        DMContainer.RegisterSingleton<IDreamineThreadScheduler>(
            new DreamineThreadScheduler());

        DMContainer.RegisterSingleton<IDreamineThreadManager>(
            new DreamineThreadManager(
                DMContainer.Resolve<IThreadCoreAllocator>(),
                DMContainer.Resolve<IThreadCyclePolicy>(),
                DMContainer.Resolve<IDreamineThreadScheduler>(),
                ResolveOptional<IThreadAffinityService>(),
                ResolveOptional<ITimerResolutionService>(),
                ResolveOptional<IDreamineLogger>()));
    }

    private static T? ResolveOptional<T>() where T : class
    {
        return DMContainer.IsRegistered<T>()
            ? DMContainer.Resolve<T>()
            : null;
    }
}
