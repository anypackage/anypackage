// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Management.Automation;

using AnyPackage.Commands.Internal;
using AnyPackage.Provider;
using AnyPackage.Resources;

namespace AnyPackage.Commands;

/// <summary>
/// The Find-Package command.
/// </summary>
[Cmdlet(VerbsCommon.Find, "Package", HelpUri = "https://go.anypackage.dev/Find-Package")]
[OutputType(typeof(PackageInfo))]
public sealed class FindPackageCommand : PackageCommandBase
{
    private const PackageProviderOperations Find = PackageProviderOperations.Find;

    /// <summary>
    /// Gets or sets the name(s).
    /// </summary>
    [Parameter(Position = 0,
        ParameterSetName = Constants.NameParameterSet,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
    [SupportsWildcards]
    [ValidateNotNullOrEmpty]
    public string[] Name { get; set; } = ["*"];

    /// <summary>
    /// Gets or sets the version of the packages to retrieve.
    /// </summary>
    /// <remarks>
    /// Accepts NuGet version range syntax.
    /// </remarks>
    [Parameter(Position = 1,
        ParameterSetName = Constants.NameParameterSet)]
    [ValidateNotNullOrEmpty]
    public PackageVersionRange Version { get; set; } = new PackageVersionRange();

    /// <summary>
    /// Gets or sets the source.
    /// </summary>
    [Parameter(ValueFromPipelineByPropertyName = true,
        ParameterSetName = Constants.NameParameterSet)]
    [ValidateNotNullOrEmpty]
    [ValidateNoWildcards]
    [ArgumentCompleter(typeof(SourceArgumentCompleter))]
    [Alias("Repository")]
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets if prerelease versions should be included.
    /// </summary>
    [Parameter(ParameterSetName = Constants.NameParameterSet)]
    public SwitchParameter Prerelease { get; set; }

    /// <summary>
    /// Gets or sets the provider.
    /// </summary>
    [Parameter]
    [ValidateNotNullOrEmpty]
    [ValidateProvider(Find)]
    [ArgumentCompleter(typeof(ProviderArgumentCompleter))]
    public override string Provider { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the package path(s).
    /// </summary>
    [Parameter(Mandatory = true,
        ParameterSetName = Constants.PathParameterSet)]
    [SupportsWildcards]
    [ValidateNotNullOrEmpty]
    [Alias("FilePath")]
    public string[] Path { get; set; } = [];

    /// <summary>
    /// Gets or sets the package path(s).
    /// </summary>
    [Parameter(Mandatory = true,
        ParameterSetName = Constants.LiteralPathParameterSet)]
    [ValidateNotNullOrEmpty]
    [Alias("PSPath")]
    public string[] LiteralPath { get; set; } = [];

    /// <summary>
    /// Gets or sets the package Uri(s).
    /// </summary>
    [Parameter(Mandatory = true,
        ParameterSetName = Constants.UriParameterSet)]
    [ValidateNotNullOrEmpty]
    public Uri[] Uri { get; set; } = [];

    /// <summary>
    /// Instances the <c>FindPackageCommand</c> class.
    /// </summary>
    public FindPackageCommand()
    {
        Operation = Find;
    }

    /// <summary>
    /// Processes input.
    /// </summary>
    protected override void ProcessRecord()
    {
        switch (ParameterSetName)
        {
            case Constants.NameParameterSet:
                InvokeByName();
                break;

            case Constants.PathParameterSet:
            case Constants.LiteralPathParameterSet:
                InvokeByPath();
                break;

            case Constants.UriParameterSet:
                InvokeByUri();
                break;
        }
    }

    /// <summary>
    /// Sets the request property.
    /// </summary>
    protected override void SetRequest()
    {
        base.SetRequest();
        Request.PassThru = true;
        Request.Prerelease = Prerelease;
    }

    private IDictionary<PackageProvider, InvokePackage> GetInvoke(IEnumerable<PackageProvider> instances)
    {
        var dictionary = new Dictionary<PackageProvider, InvokePackage>();

        foreach (var instance in instances)
        {
            dictionary.Add(instance, instance.FindPackage);
        }

        return dictionary;
    }

    private void InvokeByName()
    {
        PackageVersionRange? version = MyInvocation.BoundParameters.ContainsKey(nameof(Version)) ? Version : null;
        string? source = MyInvocation.BoundParameters.ContainsKey(nameof(Source)) ? Source : null;
        var instances = GetNameInstances();

        if (source is not null)
        {
            instances = FilterSource(source, instances);
        }

        var invoke = GetInvoke(instances);

        foreach (var name in Name)
        {
            SetRequest(name, version, source);
            Invoke(name, Strings.Finding, invoke);
        }
    }

    private void InvokeByPath()
    {
        IEnumerable<string> paths;

        if (ParameterSetName == Constants.PathParameterSet)
        {
            paths = GetPaths(Path, true);
        }
        else
        {
            paths = GetPaths(LiteralPath, false);
        }

        foreach (var path in paths)
        {
            var instances = GetPathInstances(path);
            var invoke = GetInvoke(instances);

            SetPathRequest(path);
            Invoke(path, Strings.Finding, invoke);
        }
    }

    private void InvokeByUri()
    {
        foreach (var uri in Uri)
        {
            var instances = GetUriInstances(uri);
            var invoke = GetInvoke(instances);

            SetRequest(uri);
            Invoke(uri.ToString(), Strings.Finding, invoke);
        }
    }
}
