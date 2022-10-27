// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Management.Automation;
using AnyPackage.Commands.Internal;
using AnyPackage.Provider;

namespace AnyPackage.Commands
{
    [Cmdlet(VerbsLifecycle.Unregister, "PackageSource",
            SupportsShouldProcess = true,
            HelpUri = "https://go.anypackage.dev/Unregister-PackageSource")]
    [OutputType(typeof(PackageSourceInfo))]
    public sealed class UnregisterPackageSourceCommand : SourceCommandBase
    {
        private const PackageProviderOperations SetSource = PackageProviderOperations.SetSource;

        /// <summary>
        /// Gets or sets the provider name(s).
        /// </summary>
        [Parameter(Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string[] Name { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        [Parameter]
        [ValidateNotNullOrEmpty]
        [ValidateProvider(SetSource)]
        [ArgumentCompleter(typeof(ProviderArgumentCompleter))]
        public override string Provider { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets if the command should pass objects through.
        /// </summary>
        [Parameter]
        public SwitchParameter PassThru { get; set; }

        public UnregisterPackageSourceCommand()
        {
            Operation = SetSource;
        }

        protected override void ProcessRecord()
        {
            var instances = GetInstances(Provider);

            foreach (var name in Name)
            {
                if (!ShouldProcess(name))
                {
                    continue;
                }

                WriteVerbose($"Unregistering '{name}' source.");

                SetRequest(name);

                foreach (var instance in instances)
                {
                    WriteVerbose($"Calling '{instance.ProviderInfo.Name}' provider.");
                    Request.ProviderInfo = instance.ProviderInfo;

                    try
                    {
                        instance.UnregisterSource(Request);
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

                    // Only unregister first source found.
                    if (Request.HasWriteObject)
                    {
                        break;
                    }
                }

                if (!Request.HasWriteObject)
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
            Request.PassThru = PassThru;
        }
    }
}
