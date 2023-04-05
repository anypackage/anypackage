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
    /// The Update-Package command.
    /// </summary>
    [Cmdlet(VerbsData.Update, "Package",
            SupportsShouldProcess = true,
            DefaultParameterSetName = Constants.NameParameterSet,
            HelpUri = "https://go.anypackage.dev/Update-Package")]
    [OutputType(typeof(PackageInfo))]
    public sealed class UpdatePackageCommand : PackageCommandBase
    {
        private const PackageProviderOperations Update = PackageProviderOperations.Update;
        private const string Updating = "Updating";

        /// <summary>
        /// Gets or sets the name(s).
        /// </summary>
        [Parameter(ParameterSetName = Constants.NameParameterSet,
            Position = 0,
            ValueFromPipeline = true)]
        [SupportsWildcards]
        [ValidateNotNullOrEmpty]
        public string[] Name { get; set; } = new string[] { "*" };

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
        [Parameter(ParameterSetName = Constants.UriParameterSet)]
        [ValidateNotNullOrEmpty]
        [ValidateProvider(Update)]
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
        /// Gets or sets the package Uri(s).
        /// </summary>
        [Parameter(Mandatory = true,
            ParameterSetName = Constants.UriParameterSet)]
        [ValidateNotNullOrEmpty]
        public Uri[] Uri { get; set; } = Array.Empty<Uri>();

        /// <summary>
        /// Instantiates the <c>UpdatePackageCommand</c> class.
        /// </summary>
        public UpdatePackageCommand()
        {
            Operation = Update;
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
                var instances = GetInstances(Provider);

                if (source is not null)
                {
                    instances = FilterSource(source, instances);
                }

                var invoke = GetInvoke(instances);

                foreach (var name in Name)
                {
                    SetRequest(name, version, source, TrustSource);
                    Invoke(name, Updating, invoke, true);
                }
            }
            else if (ParameterSetName == Constants.InputObjectParameterSet)
            {
                foreach (var package in InputObject)
                {
                    if (!ValidateOperation(package, PackageProviderOperations.Update))
                    {
                        continue;
                    }

                    var instances = GetInstances(package.Provider.FullName);
                    var invoke = GetInvoke(instances);
                    SetRequest(package, TrustSource);
                    Invoke(package.Name, Updating, invoke, true);
                }
            }
            else if (ParameterSetName == Constants.PathParameterSet
                     || ParameterSetName == Constants.LiteralPathParameterSet)
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
                    Invoke(path, Updating, invoke, true);
                }
            }
            else if (ParameterSetName == Constants.UriParameterSet)
            {
                foreach (var uri in Uri)
                {
                    var instances = GetUriInstances(uri);
                    var invoke = GetInvoke(instances);

                    SetRequest(uri);
                    Invoke(uri.ToString(), Updating, invoke, true);
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

        private IDictionary<PackageProvider, InvokePackage> GetInvoke(IEnumerable<PackageProvider> instances)
        {
            var dictionary = new Dictionary<PackageProvider, InvokePackage>();

            foreach (var instance in instances)
            {
                dictionary.Add(instance, instance.UpdatePackage);
            }

            return dictionary;
        }
    }
}
