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
    /// The Install-Package command.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Install, "Package",
            SupportsShouldProcess = true,
            DefaultParameterSetName = Constants.NameParameterSet,
            HelpUri = "https://go.anypackage.dev/Install-Package")]
    [OutputType(typeof(PackageInfo))]
    public sealed class InstallPackageCommand : PackageCommandBase
    {
        private const PackageProviderOperations Install = PackageProviderOperations.Install;

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
        [Parameter(ParameterSetName = Constants.NameParameterSet)]
        [Parameter(ParameterSetName = Constants.InputObjectParameterSet)]
        [Alias("TrustRepository")]
        public SwitchParameter TrustSource { get; set; }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        [Parameter(ParameterSetName = Constants.NameParameterSet)]
        [Parameter(ParameterSetName = Constants.PathParameterSet)]
        [Parameter(ParameterSetName = Constants.LiteralPathParameterSet)]
        [ValidateNotNullOrEmpty]
        [ValidateProvider(Install)]
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
        /// Gets or sets the package path(s).
        /// </summary>
        [Parameter(Mandatory = true,
            ParameterSetName = Constants.PathParameterSet)]
        [SupportsWildcards]
        [ValidateNotNullOrEmpty]
        [Alias("FilePath")]
        public string[] Path { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the package path(s).
        /// </summary>
        [Parameter(Mandatory = true,
            ParameterSetName = Constants.LiteralPathParameterSet)]
        [ValidateNotNullOrEmpty]
        [Alias("PSPath")]
        public string[] LiteralPath { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Instantiates the <c>InstallPackageCommand</c> class.
        /// </summary>
        public InstallPackageCommand()
        {
            Operation = Install;
        }

        /// <summary>
        /// Processes input.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (ParameterSetName == Constants.NameParameterSet)
            {
                PackageVersionRange? version = MyInvocation.BoundParameters.ContainsKey(nameof(Version)) ? Version : null;
                string? source = MyInvocation.BoundParameters.ContainsKey(nameof(Source)) ? Source : null;
                var instances = GetNameInstances(Provider);

                foreach (var name in Name)
                {
                    SetRequest(name, version, source, TrustSource);
                    InstallPackage(name, instances);
                }
            }
            else if (ParameterSetName == Constants.InputObjectParameterSet)
            {
                foreach (var package in InputObject)
                {
                    if (!ValidateOperation(package, PackageProviderOperations.Install))
                    {
                        continue;
                    }

                    var instances = GetInstances(package.Provider.FullName);
                    SetRequest(package, TrustSource);
                    InstallPackage(package.Name, instances);
                }
            }
            else if (ParameterSetName == Constants.PathParameterSet)
            {
                foreach (var path in GetPaths(Path, true))
                {
                    var instances = GetPathInstances(path);

                    if (instances.Count > 0)
                    {
                        SetPathRequest(path);
                        InstallPackage(path, instances);
                    }
                }
            }
            else if (ParameterSetName == Constants.LiteralPathParameterSet)
            {
                foreach (var path in GetPaths(LiteralPath, false))
                {
                    var instances = GetPathInstances(path);
                    
                    if (instances.Count > 0)
                    {
                        SetPathRequest(path);
                        InstallPackage(path, instances);
                    }
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
        }

        private void InstallPackage(string package, IEnumerable<PackageProvider> instances)
        {
            if (!ShouldProcess(package))
            {
                return;
            }

            WriteVerbose($"Installing '{package}' package.");

            foreach (var instance in instances)
            {
                WriteVerbose($"Calling '{instance.ProviderInfo.Name}' provider.");
                Request.ProviderInfo = instance.ProviderInfo;

                try
                {
                    instance.InstallPackage(Request);
                }
                catch (PipelineStoppedException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    var ex = new PackageProviderException(e.Message, e);
                    var er = new ErrorRecord(ex, "PackageProviderError", ErrorCategory.NotSpecified, package);
                    WriteError(er);
                }

                // Only install first package found.
                if (Request.HasWriteObject)
                {
                    break;
                }
            }

            if (!Request.HasWriteObject)
            {
                var ex = new PackageNotFoundException(package);
                var err = new ErrorRecord(ex, "PackageNotFound", ErrorCategory.ObjectNotFound, package);
                WriteError(err);
            }
        }
    }
}
