// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using NuGet.Versioning;
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

        protected virtual void SetRequest()
        {
            Request.DynamicParameters = DynamicParameters;
            Request.HasWriteObject = false;
        }

        protected virtual void SetRequest(string name,
                                          VersionRange? version = null,
                                          string? source = null,
                                          bool trustSource = false)
        {
            SetRequest();
            Request.Name = name;
            Request.Version = version;
            Request.Source = source;
            Request.TrustSource = trustSource;
        }

        protected virtual void SetRequest(PackageInfo package, bool trustSource = false)
        {
            SetRequest();
            Request.Name = package.Name;
            Request.Source = package.Source?.Name;
            Request.Package = package;
            Request.Prerelease = package.Version.IsPrerelease;
            Request.ProviderInfo = package.Provider;
            Request.TrustSource = trustSource;
            Request.Version = VersionRange.Parse($"[{package.Version}]");
        }
    }
}
