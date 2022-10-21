// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace UniversalPackageManager.Provider
{
    /// <summary>
    /// This class is used to manage package providers.
    /// </summary>
    public static class PackageProviderManager
    {
        private static ImmutableDictionary<Guid, PackageProviderInfo> Providers = ImmutableDictionary<Guid, PackageProviderInfo>.Empty;

        /// <summary>
        /// Register a package provider.
        /// </summary>
        /// <param name="type">The type implementing the package provider.</param>
        public static void RegisterProvider(Type type)
        {
            var providerInfo = new PackageProviderInfo(type);

            if (Providers.ContainsKey(providerInfo.Id))
            {
                return;
            }

            InitializeProvider(providerInfo);
        }

        /// <summary>
        /// Register a package provider.
        /// </summary>
        /// <param name="type">The type implementing the package provider.</param>
        /// <param name="module">The module associated with the package provider.</param>
        public static void RegisterProvider(Type type, PSModuleInfo module)
        {
            var providerInfo = new PackageProviderInfo(type, module);

            if (Providers.ContainsKey(providerInfo.Id))
            {
                return;
            }

            InitializeProvider(providerInfo);
        }

        /// <summary>
        /// Register a package provider.
        /// </summary>
        /// <param name="type">The type implementing the package provider.</param>
        /// <param name="moduleName">The module name associated with the package provider.</param>
        public static void RegisterProvider(Type type, string moduleName)
        {
            var module = GetModuleInfo(moduleName);
            var providerInfo = module is not null ?
                new PackageProviderInfo(type, module) :
                new PackageProviderInfo(type, moduleName);

            if (Providers.ContainsKey(providerInfo.Id))
            {
                return;
            }

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


        /// <summary>
        /// Unregister a package provider.
        /// </summary>
        /// <param name="type">The type implementing the package provider.</param>
        public static void UnregisterProvider(Type type)
        {
            var providerInfo = new PackageProviderInfo(type);

            if (!Providers.ContainsKey(providerInfo.Id))
            {
                return;
            }

            CleanProvider(Providers[providerInfo.Id]);
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
            provider.Id = instance.Id;
            instance.Initialize();

            Providers = Providers.Add(provider.Id, provider);
        }

        private static void CleanProvider(PackageProviderInfo provider)
        {
            var instance = provider.CreateInstance();
            instance.Clean();

            Providers = Providers.Remove(provider.Id);
        }

        private static EngineIntrinsics GetExecutionContext()
        {
            var engine = PowerShell.Create(RunspaceMode.CurrentRunspace)
                                   .AddScript("$ExecutionContext")
                                   .Invoke<EngineIntrinsics>()
                                   .First();

            return engine;
        }

        private static PSModuleInfo? GetModuleInfo(string name)
        {
            var module = PowerShell.Create(RunspaceMode.CurrentRunspace)
                                   .AddCommand("Get-Module")
                                   .AddParameter("Name", name)
                                   .Invoke<PSModuleInfo>()
                                   .First();

            return module;
        }
    }
}
