// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Management.Automation;
using AnyPackage.Commands.Internal;
using AnyPackage.Provider;

namespace AnyPackage.Commands
{
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
        public string[] Name { get; set; } = new string[] { "*" };

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        [ValidateProvider(GetSource)]
        [ArgumentCompleter(typeof(ProviderArgumentCompleter))]
        public override string Provider { get; set; } = string.Empty;

        public GetPackageSourceCommand()
        {
            Operation = GetSource;
        }

        protected override void ProcessRecord()
        {
            var instances = GetInstances(Provider);

            foreach (var name in Name)
            {
                WriteVerbose($"Getting '{name}' source.");

                SetRequest(name);

                foreach (var instance in instances)
                {
                    WriteVerbose($"Calling '{instance.ProviderInfo.Name}' provider.");
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
                        var ex = new PackageProviderException(e.Message, e);
                        var er = new ErrorRecord(ex, "PackageProviderError", ErrorCategory.NotSpecified, name);
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

        protected override void SetRequest()
        {
            base.SetRequest();
            Request.PassThru = true;
        }
    }
}
