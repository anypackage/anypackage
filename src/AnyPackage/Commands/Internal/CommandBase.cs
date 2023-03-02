// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Collections.Generic;
using System.Management.Automation;
using AnyPackage.Provider;
using static AnyPackage.Provider.PackageProviderManager;

namespace AnyPackage.Commands.Internal
{
    /// <summary>
    /// The base class for package and source commands.
    /// </summary>
    public abstract class CommandBase : PSCmdlet, IDynamicParameters
    {
        /// <summary>
        /// Gets or sets the package provider.
        /// </summary>
        public abstract string Provider { get; set; }

        /// <summary>
        /// Gets or sets the dynamic parameters.
        /// </summary>
        protected object? DynamicParameters { get; set; }

        /// <summary>
        /// Gets or sets package provider instances.
        /// </summary>
        protected Dictionary<Guid, PackageProvider> Instances { get; set; } = new Dictionary<Guid, PackageProvider>();

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        protected PackageProviderOperations Operation { get; set; }

        /// <summary>
        /// Returns an instance of an object that defines dynamic parameters.
        /// </summary>
        /// <returns>
        /// This method should return an object with properties with parameter attributes or a <c>RuntimeDefinedParameterDictionary</c>.
        /// </returns>
        /// <see href="link">https://learn.microsoft.com/en-us/dotnet/api/System.Management.Automation.IDynamicParameters.GetDynamicParameters</see>
        public virtual object GetDynamicParameters()
        {
            if (!string.IsNullOrEmpty(Provider))
            {
                var provider = GetProvider(Provider, Operation);
                var instance = provider.CreateInstance();
                DynamicParameters = instance.GetDynamicParameters(MyInvocation.MyCommand.Name);
            }

            return DynamicParameters!;
        }

        /// <summary>
        /// Initializes the command.
        /// </summary>
        protected override void BeginProcessing()
        {
            var providers = GetProviders(Operation);
            var noProvider = true;

            foreach (var provider in providers)
            {
                Instances[provider.Id] = provider.CreateInstance();
                noProvider = false;
            }

            if (noProvider) {
                throw new InvalidOperationException("No package provider available.");
            }
        }

        /// <summary>
        /// Returns instances of a package provider.
        /// </summary>
        /// <param name="provider">Package provider name.</param>
        /// <returns>
        /// Returns instances of a package provider.
        /// </returns>
        protected IEnumerable<PackageProvider> GetInstances(string provider)
        {
            if (!string.IsNullOrEmpty(provider))
            {
                var id = GetProvider(provider, Operation).Id;
                return new PackageProvider[] { Instances[id] };
            }
            else
            {
                return Instances.Values;
            }
        }
    }
}
