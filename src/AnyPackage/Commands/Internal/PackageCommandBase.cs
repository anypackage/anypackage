// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
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

        /// <summary>
        /// Sets the request property.
        /// </summary>
        /// <param name="path">Specifies the path.</param>
        protected virtual void SetRequest(PathInfo path)
        {
            SetRequest();
            Request.Path = path.ProviderPath;
        }

        /// <summary>
        /// Gets instances for a given path.
        /// </summary>
        /// <param name="pathInfo">The PS path.</param>
        protected IEnumerable<PackageProvider> GetInstances(PathInfo pathInfo)
        {
            var extension = Path.GetExtension(pathInfo.ProviderPath);
            var instances = GetInstances(Provider).Where(x => x.IsSupportedFileExtension(extension)).ToList();

            if (instances.Count == 0)
            {
                string message;
                if (MyInvocation.BoundParameters.ContainsKey(nameof(Provider)))
                {
                    message = $"Package provider '{Provider}' does not support '{extension}' extension.";
                }
                else
                {
                    message = $"No package providers support '{extension}' extension.";
                }

                var ex = new InvalidOperationException(message);
                var er = new ErrorRecord(ex, "PackageProviderExtensionNotSupported", ErrorCategory.InvalidOperation, pathInfo);
                WriteError(er);
            }

            return instances;
        }

        /// <summary>
        /// Validates the PS path is a file path.
        /// </summary>
        /// <param name="path">The PS path.</param>
        protected bool ValidateFile(PathInfo path)
        {
            if (path.Provider.Name != "FileSystem")
            {
                var ex = new InvalidOperationException($"Path '{path}' is not a file system path.");
                var er = new ErrorRecord(ex, "PathNotFileSystemProvider", ErrorCategory.InvalidArgument, path);
                WriteError(er);
                return false;
            }

            if (!File.Exists(path.ProviderPath))
            {
                var ex = new InvalidOperationException($"Path '{path}' is not a file.");
                var er = new ErrorRecord(ex, "PathNotFile", ErrorCategory.InvalidArgument, path);
                WriteError(er);
                return false;
            }

            return true;
        }
    }
}
