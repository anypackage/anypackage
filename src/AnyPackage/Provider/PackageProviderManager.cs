// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace AnyPackage.Provider
{
    /// <summary>
    /// This class is used to manage package providers.
    /// </summary>
    public static class PackageProviderManager
    {
        [ThreadStatic]
        private static Dictionary<Guid, PackageProviderInfo>? t_providers;

        internal static Dictionary<Guid, PackageProviderInfo> Providers
        {
            get
            {
                t_providers ??= [];

                return t_providers;
            }
        }

        /// <summary>
        /// Register a package provider.
        /// </summary>
        /// <param name="id">The package provider ID.</param>
        /// <param name="type">The type implementing the package provider.</param>
        /// <param name="module">The module associated with the package provider.</param>
        public static void RegisterProvider(Guid id, Type type, PSModuleInfo module)
        {
            if (Providers.ContainsKey(id))
            {
                return;
            }

            var providerInfo = new PackageProviderInfo(id, type, module);
            InitializeProvider(providerInfo);
        }

        /// <summary>
        /// Register a package provider.
        /// </summary>
        /// <param name="id">The package provider ID.</param>
        /// <param name="type">The type implementing the package provider.</param>
        /// <param name="moduleName">The module name associated with the package provider.</param>
        public static void RegisterProvider(Guid id, Type type, string moduleName)
        {
            if (Providers.ContainsKey(id))
            {
                return;
            }

            var module = GetModuleInfo(moduleName);
            var providerInfo = module is not null ?
                new PackageProviderInfo(id, type, module) :
                new PackageProviderInfo(id, type, moduleName);

            InitializeProvider(providerInfo);
        }

        /// <summary>
        /// Unregister a package provider.
        /// </summary>
        /// <param name="id">The package provider identifier.</param>
        public static void UnregisterProvider(Guid id)
        {
            if (!Providers.ContainsKey(id))
            {
                return;
            }

            CleanProvider(Providers[id]);
        }

        internal static PackageProviderInfo GetProvider(string name, PackageProviderOperations operations)
        {
            var providers = from provider in Providers.Values
                            where provider.HasOperation(operations)
                            where provider.IsMatch(name)
                            select provider;

            return providers.First();
        }

        internal static IEnumerable<PackageProviderInfo> GetProviders()
        {
            return Providers.Values;
        }

        internal static IEnumerable<PackageProviderInfo> GetProviders(string name)
        {
            var providers = from provider in Providers.Values
                            where provider.IsMatch(name)
                            orderby provider.Priority
                            orderby provider.FullName
                            select provider;

            return providers;
        }

        internal static IEnumerable<PackageProviderInfo> GetProviders(PackageProviderOperations operations)
        {
            var providers = from provider in Providers.Values
                            where provider.HasOperation(operations)
                            orderby provider.Priority
                            orderby provider.FullName
                            select provider;

            return providers;
        }

        internal static IEnumerable<PackageProviderInfo> GetProviders(string name, PackageProviderOperations operations)
        {
            var providers = from provider in Providers.Values
                            where provider.HasOperation(operations)
                            where provider.IsMatch(name)
                            orderby provider.Priority
                            orderby provider.FullName
                            select provider;

            return providers;
        }

        private static void InitializeProvider(PackageProviderInfo provider)
        {
            var instance = provider.CreateInstance();
            instance.Initialize();

            Providers.Add(provider.Id, instance.ProviderInfo);
        }

        private static void CleanProvider(PackageProviderInfo provider)
        {
            var instance = provider.CreateInstance();
            instance.Clean();

            Providers.Remove(provider.Id);
        }

        internal static PSModuleInfo? GetModuleInfo(string? name)
        {
            var module = PowerShell.Create(RunspaceMode.CurrentRunspace)
                                   .AddCommand("Get-Module")
                                   .AddParameter("Name", name)
                                   .Invoke<PSModuleInfo>()
                                   .FirstOrDefault();

            return module;
        }
    }
}
