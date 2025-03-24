// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

namespace AnyPackage.Provider;

/// <summary>
/// The supported operations for a package provider.
/// </summary>
[Flags]
public enum PackageProviderOperations
{
    /// <summary>
    /// The package provider does not support any operations.
    /// </summary>
    None = 0,

    /// <summary>
    /// The package provider supports the Find-Package command.
    /// </summary>
    Find = 1,

    /// <summary>
    /// The package provider supports the Get-Package command.
    /// </summary>
    Get = 2,

    /// <summary>
    /// The package provider supports the Publish-Package command.
    /// </summary>
    Publish = 4,

    /// <summary>
    /// The package provider supports the Install-Package command.
    /// </summary>
    Install = 8,

    /// <summary>
    /// The package provider supports the Save-Package command.
    /// </summary>
    Save = 16,

    /// <summary>
    /// The package provider supports the Uninstall-Package command.
    /// </summary>
    Uninstall = 32,

    /// <summary>
    /// The package provider supports the Unpublish-Package command.
    /// </summary>
    Unpublish = 64,

    /// <summary>
    /// The package provider supports the Update-Package command.
    /// </summary>
    Update = 128,

    /// <summary>
    /// The package provider supports the Get-PackageSource command.
    /// </summary>
    GetSource = 256,

    /// <summary>
    /// The package provider supports the Register-PackageSource,
    /// Set-PackageSource, and Unregister-PackageSource command.
    /// </summary>
    SetSource = 512,

    /// <summary>
    /// The package provider supports the Optimize-Package command.
    /// </summary>
    Optimize = 1024

    // potential additions new, build, restore (sync)
}
