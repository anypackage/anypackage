// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Management.Automation;

using AnyPackage.Commands.Internal;
using AnyPackage.Provider;
using AnyPackage.Resources;

namespace AnyPackage.Commands;

/// <summary>
/// The Publish-Package command.
/// </summary>
[Cmdlet(VerbsData.Publish, "Package",
        SupportsShouldProcess = true,
        HelpUri = "https://go.anypackage.dev/Publish-Package")]
[OutputType(typeof(PackageInfo))]
public sealed class PublishPackageCommand : PackageCommandBase
{
    private const PackageProviderOperations Publish = PackageProviderOperations.Publish;

    /// <summary>
    /// Gets or sets destination path.
    /// </summary>
    [Parameter(Mandatory = true,
        Position = 0,
        ValueFromPipeline = true)]
    [ValidateNotNullOrEmpty]
    [ValidateNoWildcards]
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the provider.
    /// </summary>
    [Parameter(Mandatory = true,
        Position = 1)]
    [ValidateNotNullOrEmpty]
    [ValidateProvider(Publish)]
    [ArgumentCompleter(typeof(ProviderArgumentCompleter))]
    public override string Provider { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the source.
    /// </summary>
    [Parameter()]
    [ValidateNotNullOrEmpty]
    [ValidateNoWildcards]
    [ArgumentCompleter(typeof(SourceArgumentCompleter))]
    [Alias("Repository")]
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets if the command should pass objects through.
    /// </summary>
    [Parameter]
    public SwitchParameter PassThru { get; set; }

    /// <summary>
    /// Instantiates the <c>PublishPackageCommand</c> class.
    /// </summary>
    public PublishPackageCommand()
    {
        Operation = Publish;
    }

    /// <summary>
    /// Processes input.
    /// </summary>
    protected override void ProcessRecord()
    {
        string? source = MyInvocation.BoundParameters.ContainsKey(nameof(Source)) ? Source : null;
        var instance = GetInstances(Provider);
        var invoke = GetInvoke(instance);

        SetRequest(Path, source);
        Invoke(Path, Strings.Publishing, invoke, true);
    }

    private void SetRequest(string path, string? source)
    {
        base.SetRequest();
        Request.Path = path;
        Request.Source = source;
        Request.PassThru = PassThru;
    }

    private IDictionary<PackageProvider, InvokePackage> GetInvoke(IEnumerable<PackageProvider> instances)
    {
        var dictionary = new Dictionary<PackageProvider, InvokePackage>();

        foreach (var instance in instances)
        {
            dictionary.Add(instance, instance.PublishPackage);
        }

        return dictionary;
    }
}
