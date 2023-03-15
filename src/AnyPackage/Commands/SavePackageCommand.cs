// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Collections.Generic;
using System.Management.Automation;
using AnyPackage.Commands.Internal;
using AnyPackage.Provider;

namespace AnyPackage.Commands
{
    /// <summary>
    /// The Save-Package command.
    /// </summary>
    [Cmdlet(VerbsData.Save, "Package",
            SupportsShouldProcess = true,
            ConfirmImpact = ConfirmImpact.Low,
            DefaultParameterSetName = Constants.NameParameterSet,
            HelpUri = "https://go.anypackage.dev/Save-Package")]
    [OutputType(typeof(PackageInfo))]
    public sealed class SavePackageCommand : PackageCommandBase
    {
        private const PackageProviderOperations Save = PackageProviderOperations.Save;

        /// <summary>
        /// Gets or sets the name(s).
        /// </summary>
        [Parameter(Mandatory = true,
            ParameterSetName = Constants.NameParameterSet,
            Position = 0,
            ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        [ValidateNoWildcards]
        public string[] Name { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the version of the packages to retrieve.
        /// </summary>
        /// <remarks>
        /// Accepts NuGet version range syntax.
        /// </remarks>
        [Parameter(ParameterSetName = Constants.NameParameterSet,
            Position = 1)]
        [ValidateNotNullOrEmpty]
        public PackageVersionRange Version { get; set; } = new PackageVersionRange();

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        [Parameter(ParameterSetName = Constants.NameParameterSet)]
        [ValidateNotNullOrEmpty]
        [ValidateNoWildcards]
        [ArgumentCompleter(typeof(SourceArgumentCompleter))]
        [Alias("Repository")]
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets destination path.
        /// </summary>
        [Parameter]
        [Alias("DestinationPath")]
        [ValidateNotNullOrEmpty]
        [ValidatePathIsDirectory]
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets if prerelease versions should be included.
        /// </summary>
        [Parameter(ParameterSetName = Constants.NameParameterSet)]
        public SwitchParameter Prerelease { get; set; }

        /// <summary>
        /// Gets or sets if the command should pass objects through.
        /// </summary>
        [Parameter]
        public SwitchParameter PassThru { get; set; }

        /// <summary>
        /// Gets or sets an untrusted source to trusted for this execution.
        /// </summary>
        [Parameter]
        [Alias("TrustRepository")]
        public SwitchParameter TrustSource { get; set; }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        [Parameter()]
        [ValidateNotNullOrEmpty]
        [ValidateProvider(Save)]
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

        /// <summary>
        /// Instantiates the <c>SavePackageCommand</c> class.
        /// </summary>
        public SavePackageCommand()
        {
            Operation = Save;
        }

        /// <summary>
        /// Initializes the command.
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (!MyInvocation.BoundParameters.ContainsKey(nameof(Path)))
            {
                Path = SessionState.Path.CurrentFileSystemLocation.Path;
            }
        }

        /// <summary>
        /// Processes input.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (ParameterSetName == Constants.NameParameterSet)
            {
                var instances = GetInstances(Provider);

                PackageVersionRange? version = MyInvocation.BoundParameters.ContainsKey(nameof(Version)) ? Version : null;
                string? source = MyInvocation.BoundParameters.ContainsKey(nameof(Source)) ? Source : null;

                foreach (var name in Name)
                {
                    SetRequest(name, version, source, TrustSource);
                    SavePackage(instances);
                }
            }
            else if (ParameterSetName == Constants.InputObjectParameterSet)
            {
                foreach (var package in InputObject)
                {
                    var instances = GetInstances(package.Provider.FullName);
                    SetRequest(package, TrustSource);
                    SavePackage(instances);
                }
            }
        }

        /// <summary>
        /// Sets the request property.
        /// </summary>
        protected override void SetRequest()
        {
            base.SetRequest();
            Request.PassThru = PassThru;
            Request.Prerelease = Prerelease;
            Request.Path = Path;
        }

        private void SavePackage(IEnumerable<PackageProvider> instances)
        {
            if (!ShouldProcess(Request.Name))
            {
                return;
            }

            WriteVerbose($"Saving '{Request.Name}' package.");

            foreach (var instance in instances)
            {
                WriteVerbose($"Calling '{instance.ProviderInfo.Name}' provider.");
                Request.ProviderInfo = instance.ProviderInfo;

                try
                {
                    instance.SavePackage(Request);
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

                // Only save first package found.
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
