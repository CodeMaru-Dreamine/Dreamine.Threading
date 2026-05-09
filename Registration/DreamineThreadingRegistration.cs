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

        DMContainer.RegisterSingleton<IThreadCoreAllocator>(
            new AutoCoreAllocator());

        DMContainer.RegisterSingleton<IThreadCyclePolicy>(
            new AdaptiveCpuCyclePolicy(
                DMContainer.Resolve<ICpuUsageProvider>()));

        DMContainer.RegisterSingleton<IDreamineThreadScheduler>(
            new DreamineThreadScheduler());

        DMContainer.RegisterSingleton<IDreamineThreadManager>(
            new DreamineThreadManager(
                DMContainer.Resolve<IThreadCoreAllocator>(),
                DMContainer.Resolve<IThreadCyclePolicy>(),
                DMContainer.Resolve<IDreamineThreadScheduler>(),
                DMContainer.Resolve<IThreadAffinityService>(),
                DMContainer.Resolve<ITimerResolutionService>(),
                DMContainer.Resolve<IDreamineLogger>()));
    }
}