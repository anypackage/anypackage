// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using AnyPackage.Provider;

namespace AnyPackage.Commands.Internal
{
    /// <summary>
    /// The base class for package commands.
    /// </summary>
    public abstract class PackageCommandBase : CommandBase
    {
        /// <summary>
        /// Gets or sets the package request.
        /// </summary>
        protected PackageRequest Request
        {
            get
            {
                _request = _request ?? new PackageRequest(this);
                return _request;
            }
        }

        private PackageRequest? _request;

        /// <summary>
        /// Sets the request property.
        /// </summary>
        protected virtual void SetRequest()
        {
            Request.DynamicParameters = DynamicParameters;
            Request.HasWriteObject = false;
        }

        /// <summary>
        /// Sets the request property.
        /// </summary>
        /// <param name="name">Specifies the package name.</param>
        /// <param name="version">Specifies the package version.</param>
        /// <param name="source">Specifies the package source.</param>
        /// <param name="trustSource">Specifies if the source should be trusted.</param>
        protected virtual void SetRequest(string name,
                                          PackageVersionRange? version = null,
                                          string? source = null,
                                          bool trustSource = false)
        {
            SetRequest();
            Request.Name = name;
            Request.Version = version;
            Request.Source = source;
            Request.TrustSource = trustSource;
        }

        /// <summary>
        /// Sets the request property.
        /// </summary>
        /// <param name="package">Specifies the package.</param>
        /// <param name="trustSource">Specifies if the source should be trusted.</param>
        protected virtual void SetRequest(PackageInfo package, bool trustSource = false)
        {
            SetRequest();
            Request.Name = package.Name;
            Request.Source = package.Source?.Name;
            Request.Package = package;
            Request.Prerelease = package.Version?.IsPrerelease ?? false;
            Request.ProviderInfo = package.Provider;
            Request.TrustSource = trustSource;
            Request.Version = PackageVersionRange.Parse($"[{package.Version}]");
        }
    }
}
