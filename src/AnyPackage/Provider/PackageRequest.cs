// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Management.Automation;

using AnyPackage.Resources;

namespace AnyPackage.Provider;

/// <summary>
/// The <c>PackageRequest</c> class is used
/// to send information to the package provider.
/// </summary>
public sealed class PackageRequest : Request
{
    /// <summary>
    /// Gets the package name.
    /// </summary>
    public string Name { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets the package version range.
    /// </summary>
    public PackageVersionRange? Version { get; internal set; }

    /// <summary>
    /// Gets the package source name.
    /// </summary>
    public string? Source { get; internal set; }

    /// <summary>
    /// Gets if should include prerelease versions.
    /// </summary>
    public bool Prerelease { get; internal set; }

    /// <summary>
    /// Gets the package if passed in via <c>InputObject</c> parameter.
    /// </summary>
    public PackageInfo? Package { get; internal set; }

    /// <summary>
    /// Gets the path parameter.
    /// </summary>
    public string Path { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets Uri parameter.
    /// </summary>
    public Uri? Uri { get; internal set; }

    /// <summary>
    /// Gets if Version is filtered.
    /// </summary>
    public bool IsVersionFiltered
    {
        get
        {
            return !(Version is null || Version.ToString() == "*");
        }
    }

    internal bool PassThru { get; set; }
    internal bool TrustSource { get; set; }

    private readonly Dictionary<Guid, HashSet<string>> _trustedRepositories = [];
    private bool _yesToAll;
    private bool _noToAll;

    internal PackageRequest(PSCmdlet command) : base(command) { }

    /// <summary>
    /// Checks if the name satisfies the request.
    /// </summary>
    /// <returns>
    /// Returns true if the name is a wildcard match to the request.
    /// </returns>
    /// <remarks>
    /// Case is ignored during comparison.
    /// </remarks>
    /// <param name="name">Specifies the name.</param>
    public bool IsMatch(string name)
    {
        var wildcardPattern = WildcardPattern.Get(Name, WildcardOptions.IgnoreCase);
        return wildcardPattern.IsMatch(name);
    }

    /// <summary>
    /// Checks if the version satisfies the request.
    /// </summary>
    /// <param name="version">Specifies the version.</param>
    /// <returns>
    /// Returns true if the version satisfies the version range requirements.
    /// </returns>
    public bool IsMatch(PackageVersion version)
    {
        if (version.IsPrerelease && !Prerelease)
        {
            return false;
        }

        if (Version is null)
        {
            return true;
        }

        return Version.Satisfies(version);
    }

    /// <summary>
    /// Checks if the version satisfies the request.
    /// </summary>
    /// <param name="version">Specifies the version.</param>
    /// <param name="comparer">Specifies the version comparer.</param>
    /// <returns>
    /// Returns true if the version satisfies the version range requirements.
    /// </returns>
    public bool IsMatch(PackageVersion version, IComparer<PackageVersion> comparer)
    {
        if (version.IsPrerelease && !Prerelease)
        {
            return false;
        }

        if (Version is null)
        {
            return true;
        }

        return Version.Satisfies(version, comparer);
    }

    /// <summary>
    /// Checks if the package name and version satisfies the request.
    /// </summary>
    /// <param name="name">Specifies the name.</param>
    /// <param name="version">Specifies the version.</param>
    /// <returns>
    /// Returns true if the name and version satisfies the request.
    /// </returns>
    public bool IsMatch(string name, PackageVersion version)
    {
        return IsMatch(name) && IsMatch(version);
    }

    /// <summary>
    /// Prompts the user if they want to install a package from an untrusted source.
    /// </summary>
    /// <param name="source">Source name.</param>
    /// <returns>
    /// Returns true if the user accepted or false if the user rejected.
    /// </returns>
    public bool PromptUntrustedSource(string source)
    {
        if (TrustSource) { return true; }
        if (_yesToAll) { return true; }
        if (_noToAll) { return false; }

        if (ProviderInfo is not null &&
            _trustedRepositories.TryGetValue(ProviderInfo.Id, out var trustedRepositories) &&
            trustedRepositories.Contains(source))
        {
            return true;
        }

        var trusted = Cmdlet.ShouldContinue(
            query: string.Format(Strings.InstallFromUntrustedSource, source),
            caption: Strings.UntrustedSource,
            hasSecurityImpact: true,
            yesToAll: ref _yesToAll,
            noToAll: ref _noToAll
        );

        if (trusted && ProviderInfo is not null)
        {
            if (!_trustedRepositories.ContainsKey(ProviderInfo.Id))
            {
                _trustedRepositories[ProviderInfo.Id] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            _trustedRepositories[ProviderInfo.Id].Add(source);
        }

        return trusted;
    }

    /// <summary>
    /// Writes the package to the pipeline.
    /// </summary>
    /// <param name="package">The package.</param>
    public void WritePackage(PackageInfo package)
    {
        HasWriteObject = true;

        if (PassThru)
        {
            Cmdlet.WriteObject(package);
        }
    }
}
