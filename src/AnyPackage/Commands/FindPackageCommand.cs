// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using AnyPackage.Commands.Internal;
using AnyPackage.Provider;

namespace AnyPackage.Commands
{
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
        public string[] Name { get; set; } = new string[] { "*" };

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
        [ValidateNotNullOrEmpty]
        public string[] Path { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Instances the <c>FindPackageCommand</c> class.
        /// </summary>
        public FindPackageCommand()
        {
            Operation = Find;
        }

        /// <summary>
        /// Initializes the command.
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            Request.DynamicParameters = DynamicParameters;
            Request.PassThru = true;
        }

        /// <summary>
        /// Processes input.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (ParameterSetName == Constants.NameParameterSet)
            {
                var instances = GetInstances(Provider).Where(x => x.ProviderInfo.PackageByName).ToList();

                if (instances.Count == 0)
                {
                    string message;
                    if (MyInvocation.BoundParameters.ContainsKey(nameof(Provider)))
                    {
                        message = $"Package provider '{Provider}' does not support package by name.";
                    }
                    else
                    {
                        message = $"No package providers support package by name.";
                    }

                    var ex = new InvalidOperationException(message);
                    var er = new ErrorRecord(ex, "PackageProviderNameNotSupported", ErrorCategory.InvalidOperation, Provider);
                    WriteError(er);
                }

                PackageVersionRange? version = MyInvocation.BoundParameters.ContainsKey(nameof(Version)) ? Version : null;
                string? source = MyInvocation.BoundParameters.ContainsKey(nameof(Source)) ? Source : null;

                foreach (var name in Name)
                {
                    SetRequest(name, version, source);
                    FindPackage(name, instances);
                }
            }
            else if (ParameterSetName == Constants.PathParameterSet)
            {
                ProcessPath();
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

        private void ProcessPath()
        {
            foreach (var path in Path)
            {
                try
                {
                    var pathInfos = SessionState.Path.GetResolvedPSPathFromPSPath(path);
                    foreach (var pathInfo in pathInfos)
                    {
                        if (!ValidateFile(pathInfo))
                        {
                            continue;
                        }

                        var instances = GetInstances(pathInfo);
                        SetRequest(pathInfo);
                        FindPackage(pathInfo.Path, instances);
                    }
                }
                catch (ItemNotFoundException e)
                {
                    var ex = new PackageProviderException(e.Message, e);
                    var er = new ErrorRecord(ex, "PathNotFound", ErrorCategory.ObjectNotFound, path);
                    WriteError(er);
                }
            }
        }

        private void FindPackage(string package, IEnumerable<PackageProvider> instances)
        {
            WriteVerbose($"Finding '{package}' package.");

            foreach (var instance in instances)
            {
                WriteVerbose($"Calling '{instance.ProviderInfo.Name}' provider.");
                Request.ProviderInfo = instance.ProviderInfo;

                try
                {
                    instance.FindPackage(Request);
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
            }

            if (!Request.HasWriteObject && !WildcardPattern.ContainsWildcardCharacters(package))
            {
                var ex = new PackageNotFoundException(package);
                var er = new ErrorRecord(ex, "PackageNotFound", ErrorCategory.ObjectNotFound, package);
                WriteError(er);
            }
        }
    }
}
