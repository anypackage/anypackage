// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using AnyPackage.Provider;

namespace AnyPackage.Feedback;

/// <summary>
/// The <c>CommandNotFoundFeedback</c> class.
/// The command not found package information.
/// </summary>
/// <param name="name">Missing command package name.</param>
/// <param name="provider">Package provider.</param>
public sealed class CommandNotFoundFeedback(string name, PackageProviderInfo provider)
{
    /// <summary>
    /// Gets the missing command package name.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets if any other parameters are required to install the package.
    /// </summary>
    /// <remarks>
    /// If a specific version or package source is required add it, otherwise don't.
    /// Do not include Provider parameter as that will automatically be added.
    /// </remarks>
    public IDictionary<string, string>? RequiredParameters { get; }

    /// <summary>
    /// Gets the package provider info.
    /// </summary>
    public PackageProviderInfo Provider { get; } = provider;

    /// <summary>
    /// Instantiates a <c>PackageNotFoundException</c> class.
    /// </summary>
    /// <param name="name">Missing command package name.</param>
    /// <param name="provider">Package provider.</param>
    /// <param name="requiredParameters">Required parameters to install package.</param>
    public CommandNotFoundFeedback(string name, PackageProviderInfo provider, IDictionary<string, string> requiredParameters) : this(name, provider)
    {
        RequiredParameters = requiredParameters;
    }
}
