using System;

namespace Dreamine.Threading.Options;

/// <summary>
/// Provides configuration options for Dreamine threading registration.
/// </summary>
public sealed class DreamineThreadingOptions
{
    /// <summary>
    /// Gets or sets whether Windows-specific threading services are registered.
    /// </summary>
    public bool RegisterWindowsServices { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the adaptive CPU cycle policy is used.
    /// </summary>
    public bool UseAdaptiveCpuPolicy { get; set; } = true;

    /// <summary>
    /// Gets or sets whether an existing registration can be overwritten.
    /// </summary>
    public bool AllowOverride { get; set; } = true;
}