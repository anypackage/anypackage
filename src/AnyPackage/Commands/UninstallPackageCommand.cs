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
    /// The Uninstall-Package command.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Uninstall, "Package",
            SupportsShouldProcess = true,
            DefaultParameterSetName = Constants.NameParameterSet,
            HelpUri = "https://go.anypackage.dev/Uninstall-Package")]
    [OutputType(typeof(PackageInfo))]
    public sealed class UninstallPackageCommand : PackageCommandBase
    {
        private const PackageProviderOperations Uninstall = PackageProviderOperations.Uninstall;
        private const string Uninstalling = "Uninstalling";

        /// <summary>
        /// Gets or sets the name(s).
        /// </summary>
        [Parameter(Mandatory = true,
            ParameterSetName = Constants.NameParameterSet,
            Position = 0,
            ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        [ValidateNoWildcards]
        public string[] Name { get; set; } = [];

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
        /// Gets or sets if the command should pass objects through.
        /// </summary>
        [Parameter]
        public SwitchParameter PassThru { get; set; }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        [Parameter(ParameterSetName = Constants.NameParameterSet)]
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
        public PackageInfo[] InputObject { get; set; } = [];

        /// <summary>
        /// Instantiates the <c>UninstallPackageCommand</c> class.
        /// </summary>
        public UninstallPackageCommand()
        {
            Operation = Uninstall;
        }

        /// <summary>
        /// Processes input.
        /// </summary>
        protected override void ProcessRecord()
        {
            switch (ParameterSetName)
            {
                case Constants.NameParameterSet:
                    InvokeByName();
                    break;

                case Constants.InputObjectParameterSet:
                    InvokeByInputObject();
                    break;
            }
        }

        /// <summary>
        /// Sets the request property.
        /// </summary>
        protected override void SetRequest()
        {
            base.SetRequest();
            Request.PassThru = PassThru;
            Request.Prerelease = true;
        }

        private IDictionary<PackageProvider, InvokePackage> GetInvoke(IEnumerable<PackageProvider> instances)
        {
            var dictionary = new Dictionary<PackageProvider, InvokePackage>();

            foreach (var instance in instances)
            {
                dictionary.Add(instance, instance.UninstallPackage);
            }

            return dictionary;
        }

        private void InvokeByName()
        {
            PackageVersionRange? version = MyInvocation.BoundParameters.ContainsKey(nameof(Version)) ? Version : null;
            var instances = GetInstances(Provider);
            var invoke = GetInvoke(instances);

            foreach (var name in Name)
            {
                SetRequest(name, version);
                Invoke(name, Uninstalling, invoke, true, true);
            }
        }

        private void InvokeByInputObject()
        {
            foreach (var package in InputObject)
            {
                if (!ValidateOperation(package, PackageProviderOperations.Uninstall))
                {
                    continue;
                }

                var instances = GetInstances(package.Provider.FullName);
                var invoke = GetInvoke(instances);
                SetRequest(package);
                Invoke(package.Name, Uninstalling, invoke, true, true);
            }
        }
    }
}
