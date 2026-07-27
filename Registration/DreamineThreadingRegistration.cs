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
/// \if KO
/// <para>Dreamine 스레딩 핵심 서비스를 전역 컨테이너에 등록하는 도우미를 제공합니다.</para>
/// \endif
/// \if EN
/// <para>Provides helpers that register Dreamine threading core services in the global container.</para>
/// \endif
/// </summary>
public static class DreamineThreadingRegistration
{
    /// <summary>
    /// \if KO
    /// <para>지정한 옵션과 선택적 CPU 사용률 공급자로 스레딩 핵심 서비스를 등록합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Registers threading core services using the specified options and optional CPU-usage provider.</para>
    /// \endif
    /// </summary>
    /// <param name="configure">
    /// \if KO
    /// <para>등록 옵션을 수정할 선택적 구성 대리자입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The optional delegate that modifies registration options.</para>
    /// \endif
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// \if KO
    /// <para>컨테이너 서비스 확인 또는 등록이 실패할 때 발생할 수 있습니다.</para>
    /// \endif
    /// \if EN
    /// <para>May be thrown when container service resolution or registration fails.</para>
    /// \endif
    /// </exception>
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
    /// \if KO
    /// <para>선택적 구성 대리자를 적용해 Dreamine 스레딩 핵심 서비스를 등록합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Registers Dreamine threading core services after applying an optional configuration delegate.</para>
    /// \endif
    /// </summary>
    /// <param name="options">
    /// \if KO
    /// <para>서비스 등록과 주기 정책 선택 옵션입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The service-registration and cycle-policy selection options.</para>
    /// \endif
    /// </param>
    /// <param name="cpuUsageProvider">
    /// \if KO
    /// <para>적응형 주기 정책에 사용할 선택적 CPU 사용률 공급자입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The optional CPU-usage provider used by the adaptive cycle policy.</para>
    /// \endif
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// \if KO
    /// <para><paramref name="options"/>가 <see langword="null"/>일 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when <paramref name="options"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// \if KO
    /// <para>필수 컨테이너 서비스를 확인하거나 등록할 수 없을 때 발생할 수 있습니다.</para>
    /// \endif
    /// \if EN
    /// <para>May be thrown when required container services cannot be resolved or registered.</para>
    /// \endif
    /// </exception>
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

    /// <summary>
    /// \if KO
    /// <para>전역 컨테이너에 등록된 선택적 서비스를 확인합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Resolves an optional service registered in the global container.</para>
    /// \endif
    /// </summary>
    /// <typeparam name="T">
    /// \if KO
    /// <para>확인할 참조 형식 서비스입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The reference-type service to resolve.</para>
    /// \endif
    /// </typeparam>
    /// <returns>
    /// \if KO
    /// <para>등록된 서비스이며 등록되지 않았으면 <see langword="null"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The registered service, or <see langword="null"/> when it is not registered.</para>
    /// \endif
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// \if KO
    /// <para>등록된 서비스를 생성하거나 확인할 수 없을 때 발생할 수 있습니다.</para>
    /// \endif
    /// \if EN
    /// <para>May be thrown when a registered service cannot be created or resolved.</para>
    /// \endif
    /// </exception>
    private static T? ResolveOptional<T>() where T : class
    {
        return DMContainer.IsRegistered<T>()
            ? DMContainer.Resolve<T>()
            : null;
    }
}
