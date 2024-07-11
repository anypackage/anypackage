// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Collections.Generic;
using System.Management.Automation;
using AnyPackage.Commands.Internal;
using AnyPackage.Provider;
using AnyPackage.Resources;

namespace AnyPackage.Commands
{
    /// <summary>
    /// The Get-Package command.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "Package", HelpUri = "https://go.anypackage.dev/Get-Package")]
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
        public string[] Name { get; set; } = ["*"];

        /// <summary>
        /// Gets or sets the version of the packages to retrieve.
        /// </summary>
        /// <remarks>
        /// Accepts NuGet version range syntax.
        /// </remarks>
        [Parameter(Position = 1)]
        [ValidateNotNullOrEmpty]
        public PackageVersionRange Version { get; set; } = new PackageVersionRange();

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        [Parameter]
        [ValidateNotNullOrEmpty]
        [ValidateProvider(Get)]
        [ArgumentCompleter(typeof(ProviderArgumentCompleter))]
        public override string Provider { get; set; } = string.Empty;

        /// <summary>
        /// Instantiates the <c>GetPackageCommand</c> class.
        /// </summary>
        public GetPackageCommand()
        {
            Operation = Get;
        }

        /// <summary>
        /// Processes input.
        /// </summary>
        protected override void ProcessRecord()
        {
            PackageVersionRange? version = MyInvocation.BoundParameters.ContainsKey(nameof(Version)) ? Version : null;
            var instances = GetInstances(Provider);
            var invoke = GetInvoke(instances);

            foreach (var name in Name)
            {
                SetRequest(name, version);
                Invoke(name, Strings.Getting, invoke);
            }
        }

        /// <summary>
        /// Sets the request property.
        /// </summary>
        protected override void SetRequest()
        {
            base.SetRequest();
            Request.PassThru = true;
            Request.Prerelease = true;
        }

        private IDictionary<PackageProvider, InvokePackage> GetInvoke(IEnumerable<PackageProvider> instances)
        {
            var dictionary = new Dictionary<PackageProvider, InvokePackage>();

            foreach (var instance in instances)
            {
                dictionary.Add(instance, instance.GetPackage);
            }

            return dictionary;
        }
    }
}
