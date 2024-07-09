// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Linq;

namespace AnyPackage.Provider
{
    /// <summary>
    /// The <c>PackageProvider</c> class.
    /// </summary>
    public abstract class PackageProvider
    {
        /// <summary>
        /// Gets the package provider information.
        /// </summary>
        /// <remarks>
        /// If a derived type of <c>PackageProviderInfo</c> was returned from the <c>Initialize</c> method, it
        /// will be set here in all subsequent calls to the provider.
        /// </remarks>
        public PackageProviderInfo ProviderInfo
        {
            get
            {
                _providerInfo ??= new PackageProviderInfo(this.GetType());
                return _providerInfo;
            }

            internal set
            {
                _providerInfo = value;
            }
        }

        private PackageProviderInfo? _providerInfo;

        /// <summary>
        /// Performs one time initialization for the package provider during registration.
        /// </summary>
        /// <returns>
        /// Returns <c>PackageProviderInfo</c> or derived type that can be used
        /// to present new properties or methods to the user.
        /// It can also be used maintain state or cache between instances of
        /// the package provider.
        /// </returns>
        protected virtual PackageProviderInfo Initialize(PackageProviderInfo providerInfo)
        {
            return providerInfo;
        }

        internal void Initialize()
        {
            ProviderInfo = Initialize(ProviderInfo);
        }

        /// <summary>
        /// Performs one time clean up of the package provider during unregistration.
        /// </summary>
        protected internal virtual void Clean() { }

        /// <summary>
        /// Gets the dynamic parameters for command name.
        /// </summary>
        /// <param name="commandName">The cmdlet name.</param>
        /// <returns>
        /// The method can be overwritten to return an object
        /// or a <c>RuntimeDefinedParameterDictionary</c>.
        /// </returns>
        protected internal virtual object? GetDynamicParameters(string commandName)
        {
            return null;
        }

        /// <summary>
        /// Gets if the source is supported by the provider.
        /// </summary>
        /// <remarks>
        /// The default implementation returns <c>true</c>. 
        /// </remarks>
        /// <param name="source">The source parameter from cmdlets.</param>
        /// <returns>Returns <c>true</c> if source is supported.</returns>
        protected internal virtual bool IsSource(string source)
        {
            return true;
        }

        internal bool IsSupportedFileExtension(string extension)
        {
            return ProviderInfo.FileExtensions.Any(x => x.Equals(extension, StringComparison.OrdinalIgnoreCase));
        }

        internal bool IsSupportedUriScheme(string extension)
        {
            return ProviderInfo.UriSchemes.Any(x => x.Equals(extension, StringComparison.OrdinalIgnoreCase));
        }

        internal void FindPackage(PackageRequest request)
        {
            if (this is not IFindPackage findPackage)
            {
                throw new InvalidOperationException();
            }

            findPackage.FindPackage(request);
        }

        internal void GetPackage(PackageRequest request)
        {
            if (this is not IGetPackage getPackage)
            {
                throw new InvalidOperationException($"Package provider '{ProviderInfo.Name}' does not support Get-Package.");
            }

            getPackage.GetPackage(request);
        }

        internal void InstallPackage(PackageRequest request)
        {
            if (this is not IInstallPackage installPackage)
            {
                throw new InvalidOperationException();
            }

            installPackage.InstallPackage(request);
        }

        internal void SavePackage(PackageRequest request)
        {
            if (this is not ISavePackage savePackage)
            {
                throw new InvalidOperationException();
            }

            savePackage.SavePackage(request);
        }

        internal void PublishPackage(PackageRequest request)
        {
            if (this is not IPublishPackage publishPackage)
            {
                throw new InvalidOperationException();
            }

            publishPackage.PublishPackage(request);
        }

        internal void UninstallPackage(PackageRequest request)
        {
            if (this is not IUninstallPackage uninstallPackage)
            {
                throw new InvalidOperationException();
            }

            uninstallPackage.UninstallPackage(request);
        }

        internal void UpdatePackage(PackageRequest request)
        {
            if (this is not IUpdatePackage updatePackage)
            {
                throw new InvalidOperationException();
            }

            updatePackage.UpdatePackage(request);
        }

        internal void GetSource(SourceRequest request)
        {
            if (this is not IGetSource packageSource)
            {
                throw new InvalidOperationException();
            }

            packageSource.GetSource(request);
        }

        internal void RegisterSource(SourceRequest request)
        {
            if (this is not ISetSource packageSource)
            {
                throw new InvalidOperationException();
            }

            packageSource.RegisterSource(request);
        }

        internal void SetSource(SourceRequest request)
        {
            if (this is not ISetSource packageSource)
            {
                throw new InvalidOperationException();
            }

            packageSource.SetSource(request);
        }

        internal void UnregisterSource(SourceRequest request)
        {
            if (this is not ISetSource packageSource)
            {
                throw new InvalidOperationException();
            }

            packageSource.UnregisterSource(request);
        }
    }
}
