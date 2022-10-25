// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Management.Automation;
using NuGet.Versioning;
using AnyPackage.Commands.Internal;
using AnyPackage.Provider;

namespace AnyPackage.Commands
{
    [Cmdlet(VerbsCommon.Get, "Package", HelpUri = "go.anypackage.dev/Get-Package")]
    [OutputType(typeof(PackageInfo))]
    public sealed class GetPackageCommand : PackageCommandBase
    {
        private const PackageProviderOperations Get = PackageProviderOperations.Get;

        /// <summary>
        /// Gets or sets the name(s).
        /// </summary>
        [Parameter(Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [SupportsWildcards]
        [ValidateNotNullOrEmpty]
        public string[] Name { get; set; } = new string[] { "*" };

        /// <summary>
        /// Gets or sets the version of the packages to retrieve.
        /// </summary>
        /// <remarks>
        /// Accepts NuGet version range syntax.
        /// </remarks>
        [Parameter(Position = 1,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [VersionRangeTransformation]
        public VersionRange Version { get; set; } = VersionRange.All;

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        [Parameter]
        [ValidateNotNullOrEmpty]
        [ValidateProvider(Get)]
        [ArgumentCompleter(typeof(ProviderArgumentCompleter))]
        public override string Provider { get; set; } = string.Empty;

        public GetPackageCommand()
        {
            Operation = Get;
        }

        protected override void ProcessRecord()
        {
            var instances = GetInstances(Provider);

            VersionRange? version = MyInvocation.BoundParameters.ContainsKey(nameof(Version)) ? Version : null;

            foreach (var name in Name)
            {
                WriteVerbose($"Getting '{name}' package.");

                SetRequest(name, version);

                foreach (var instance in instances)
                {
                    WriteVerbose($"Calling '{instance.ProviderInfo.Name}' provider.");
                    Request.ProviderInfo = instance.ProviderInfo;

                    try
                    {
                        instance.GetPackage(Request);
                    }
                    catch (PipelineStoppedException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        var ex = new PackageProviderException(e.Message, e);
                        var er = new ErrorRecord(ex, "PackageProviderError", ErrorCategory.NotSpecified, name);
                        WriteError(er);
                    }
                }

                if (!Request.HasWriteObject && !WildcardPattern.ContainsWildcardCharacters(name))
                {
                    var ex = new PackageNotFoundException(name);
                    var err = new ErrorRecord(ex, "PackageNotFound", ErrorCategory.ObjectNotFound, name);
                    WriteError(err);
                }
            }
        }

        protected override void SetRequest()
        {
            base.SetRequest();
            Request.PassThru = true;
            Request.Prerelease = true;
        }
    }
}
