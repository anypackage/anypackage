// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Collections.Generic;
using System.Management.Automation;
using NuGet.Versioning;
using UniversalPackageManager.Commands.Internal;
using UniversalPackageManager.Provider;

namespace UniversalPackageManager.Commands
{
    [Cmdlet(VerbsLifecycle.Uninstall, "Package", SupportsShouldProcess = true)]
    [OutputType(typeof(PackageInfo))]
    public sealed class UninstallPackageCommand : PackageCommandBase
    {
        private const PackageProviderOperations Uninstall = PackageProviderOperations.Uninstall;

        /// <summary>
        /// Gets or sets the name(s).
        /// </summary>
        [Parameter(Mandatory = true,
            ParameterSetName = Constants.NameParameterSet,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string[] Name { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the version of the packages to retrieve.
        /// </summary>
        /// <remarks>
        /// Accepts NuGet version range syntax.
        /// </remarks>
        [Parameter(ParameterSetName = Constants.NameParameterSet,
            Position = 1,
            ValueFromPipelineByPropertyName = true)]
        [VersionRangeTransformation]
        public VersionRange Version { get; set; } = VersionRange.AllStable;

        /// <summary>
        /// Gets or sets if the command should pass objects through.
        /// </summary>
        [Parameter]
        public SwitchParameter PassThru { get; set; }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        [Parameter()]
        [ValidateNotNullOrEmpty]
        [ValidateProvider(Uninstall)]
        [ArgumentCompleter(typeof(ProviderArgumentCompleter))]
        public override string Provider { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets package(s).
        /// </summary>
        [Parameter(Mandatory = true,
            ParameterSetName = Constants.InputObjectParameterSet,
            Position = 0,
            ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        public PackageInfo[] InputObject { get; set; } = Array.Empty<PackageInfo>();

        public UninstallPackageCommand()
        {
            Operation = Uninstall;
        }

        protected override void ProcessRecord()
        {
            if (ParameterSetName == Constants.NameParameterSet)
            {
                var instances = GetInstances(Provider);

                VersionRange? version = MyInvocation.BoundParameters.ContainsKey(nameof(Version)) ? Version : null;

                foreach (var name in Name)
                {
                    SetRequest(name, version);
                    UninstallPackage(instances);
                }
            }
            else if (ParameterSetName == Constants.InputObjectParameterSet)
            {
                foreach (var package in InputObject)
                {
                    var instances = GetInstances(package.Provider.FullName);
                    SetRequest(package);
                    UninstallPackage(instances);
                }
            }
        }

        protected override void SetRequest()
        {
            base.SetRequest();
            Request.PassThru = PassThru;
            Request.Prerelease = true;
        }

        private void UninstallPackage(IEnumerable<PackageProvider> instances)
        {
            if (!ShouldProcess(Request.Name))
            {
                return;
            }

            WriteVerbose($"Uninstalling '{Request.Name}' package.");

            foreach (var instance in instances)
            {
                WriteVerbose($"Calling '{instance.ProviderInfo.Name}' provider.");
                Request.ProviderInfo = instance.ProviderInfo;

                try
                {
                    instance.UninstallPackage(Request);
                }
                catch (PipelineStoppedException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    var ex = new PackageProviderException(e.Message, e);
                    var er = new ErrorRecord(ex, "PackageProviderError", ErrorCategory.NotSpecified, Request.Name);
                    WriteError(er);
                }

                // Only uninstall first package found.
                if (Request.HasWriteObject)
                {
                    break;
                }
            }

            if (!Request.HasWriteObject)
            {
                var ex = new PackageNotFoundException(Request.Name);
                var err = new ErrorRecord(ex, "PackageNotFound", ErrorCategory.ObjectNotFound, Request.Name);
                WriteError(err);
            }
        }
    }
}
