// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using AnyPackage.Resources;

namespace AnyPackage.Provider;

/// <summary>
/// The <c>PackageDependency</c> class.
/// Contains information about package dependency requirements.
/// </summary>
public sealed class PackageDependency
{
    /// <summary>
    /// Gets the package name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the version range.
    /// </summary>
    public PackageVersionRange VersionRange { get; } = new PackageVersionRange();

    /// <summary>
    /// Gets the package provider.
    /// </summary>
    /// <remarks>
    /// If null the current provider should be used.
    /// </remarks>
    public PackageProviderInfo? Provider { get; }

    /// <summary>
    /// Constructs a new instance of the <c>PackageDependency</c> class.
    /// </summary>
    /// <param name="name">Package name.</param>
    public PackageDependency(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(Strings.PackageNameNullOrWhiteSpace, nameof(name));
        }

        Name = name;
    }

    /// <summary>
    /// Constructs a new instance of the <c>PackageDependency</c> class.
    /// </summary>
    /// <param name="name">Package name.</param>
    /// <param name="versionRange">Version range.</param>
    public PackageDependency(string name, PackageVersionRange versionRange) : this(name)
    {
        if (versionRange is null)
        {
            throw new ArgumentNullException(nameof(versionRange));
        }

        VersionRange = versionRange;
    }

    /// <summary>
    /// Constructs a new instance of the <c>PackageDependency</c> class.
    /// </summary>
    /// <param name="name">Package name.</param>
    /// <param name="provider">Package provider.</param>
    public PackageDependency(string name, PackageProviderInfo provider) : this(name)
    {
        if (provider is null)
        {
            throw new ArgumentNullException(nameof(provider));
        }

        Provider = provider;
    }

    /// <summary>
    /// Constructs a new instance of the <c>PackageDependency</c> class.
    /// </summary>
    /// <param name="name">Package name.</param>
    /// <param name="versionRange">Version range.</param>
    /// <param name="provider">Package provider.</param>
    public PackageDependency(string name, PackageVersionRange versionRange, PackageProviderInfo provider) : this(name, versionRange)
    {
        if (provider is null)
        {
            throw new ArgumentNullException(nameof(provider));
        }

        Provider = provider;
    }

    /// <summary>
    /// Returns a string of the package name.
    /// </summary>
    /// <returns>
    /// The package name.
    /// </returns>
    public override string ToString() => Name;
}
