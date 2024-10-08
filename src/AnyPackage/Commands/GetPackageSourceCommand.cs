// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Management.Automation;

using AnyPackage.Commands.Internal;
using AnyPackage.Provider;
using AnyPackage.Resources;

namespace AnyPackage.Commands;

/// <summary>
/// The Get-PackageSource command.
/// </summary>
[Cmdlet(VerbsCommon.Get, "PackageSource", HelpUri = "https://go.anypackage.dev/Get-PackageSource")]
[OutputType(typeof(PackageSourceInfo))]
public sealed class GetPackageSourceCommand : SourceCommandBase
{
    private const PackageProviderOperations GetSource = PackageProviderOperations.GetSource;

    /// <summary>
    /// Gets or sets the provider name(s).
    /// </summary>
    [Parameter(Position = 0,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
    [SupportsWildcards]
    [ValidateNotNullOrEmpty]
    [ArgumentCompleter(typeof(SourceArgumentCompleter))]
    public string[] Name { get; set; } = ["*"];

    /// <summary>
    /// Gets or sets the provider.
    /// </summary>
    [Parameter(ValueFromPipelineByPropertyName = true)]
    [ValidateNotNullOrEmpty]
    [ValidateProvider(GetSource)]
    [ArgumentCompleter(typeof(ProviderArgumentCompleter))]
    public override string Provider { get; set; } = string.Empty;

    /// <summary>
    /// Instantiates the <c>GetPackageSourceCommand</c> class.
    /// </summary>
    public GetPackageSourceCommand()
    {
        Operation = GetSource;
    }

    /// <summary>
    /// Processes input.
    /// </summary>
    protected override void ProcessRecord()
    {
        var instances = GetInstances(Provider);

        foreach (var name in Name)
        {
            WriteVerbose(Strings.GettingSource);

            SetRequest(name);

            foreach (var instance in instances)
            {
                WriteVerbose(string.Format(Strings.CallingProvider, instance.ProviderInfo.Name));
                Request.ProviderInfo = instance.ProviderInfo;

                try
                {
                    instance.GetSource(Request);
                }
                catch (PipelineStoppedException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    var er = new ErrorRecord(e, "PackageProviderError", ErrorCategory.NotSpecified, name);
                    WriteError(er);
                }
            }

            if (!Request.HasWriteObject && !WildcardPattern.ContainsWildcardCharacters(name))
            {
                var ex = new PackageSourceNotFoundException(name);
                var err = new ErrorRecord(ex, "PackageSourceNotFound", ErrorCategory.ObjectNotFound, name);
                WriteError(err);
            }
        }
    }

    /// <summary>
    /// Sets the request property.
    /// </summary>
    protected override void SetRequest()
    {
        base.SetRequest();
        Request.PassThru = true;
    }
}
