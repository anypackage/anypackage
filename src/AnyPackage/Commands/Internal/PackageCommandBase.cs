// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /// Invokes a package operation.
        /// </summary>
        /// <param name="request">The package request.</param>
        protected delegate void InvokePackage(PackageRequest request);

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
        /// <param name="uri">Specifies the package uri.</param>
        protected virtual void SetRequest(Uri uri)
        {
            SetRequest();
            Request.Uri = uri;
        }

        /// <summary>
        /// Sets the request property.
        /// </summary>
        /// <param name="path">Specifies the path.</param>
        protected virtual void SetPathRequest(string path)
        {
            SetRequest();
            Request.Path = path;
        }

        /// <summary>
        /// Gets provider instances.
        /// </summary>
        /// <param name="name">Name of the provider.</param>
        protected IEnumerable<PackageProvider> GetNameInstances(string name)
        {
            var instances = GetInstances(Provider).Where(x => x.ProviderInfo.PackageByName).ToList();

            if (instances.Count == 0)
            {
                string message;
                if (MyInvocation.BoundParameters.ContainsKey(nameof(Provider)))
                {
                    message = $"Package provider '{Provider}' does not support package by name.";
                }
                else
                {
                    message = $"No package providers support package by name.";
                }

                throw new InvalidOperationException(message);
            }

            return instances;
        }

        /// <summary>
        /// Gets provider instances for a given path.
        /// </summary>
        /// <param name="path">The path.</param>
        protected IEnumerable<PackageProvider> GetPathInstances(string path)
        {
            var extension = Path.GetExtension(path);
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
                var er = new ErrorRecord(ex, "PackageProviderExtensionNotSupported", ErrorCategory.InvalidOperation, path);
                WriteError(er);
            }

            return instances;
        }

        /// <summary>
        /// Get provider instances for a given Uri.
        /// </summary>
        /// <param name="uri">The Uri.</param>
        protected IEnumerable<PackageProvider> GetUriInstances(Uri uri)
        {
            var instances = GetInstances(Provider).Where(x => x.IsSupportedUriScheme(uri.Scheme)).ToList();

            if (instances.Count == 0)
            {
                string message;
                if (MyInvocation.BoundParameters.ContainsKey(nameof(Provider)))
                {
                    message = $"Package provider '{Provider}' does not support '{uri.Scheme}' scheme.";
                }
                else
                {
                    message = $"No package providers support '{uri.Scheme}' scheme.";
                }

                var ex = new InvalidOperationException(message);
                var er = new ErrorRecord(ex, "PackageProviderSchemeNotSupported", ErrorCategory.InvalidOperation, uri);
                WriteError(er);
            }

            return instances;
        }

        /// <summary>
        /// Gets normalized paths.
        /// </summary>
        /// <param name="paths">The paths to normalize.</param>
        /// <param name="expandWildcards">If wildcards should be expanded.</param>
        protected IEnumerable<string> GetPaths(IEnumerable<string> paths, bool expandWildcards)
        {
            var filePaths = new List<string>();

            if (expandWildcards)
            {
                foreach (var path in paths)
                {
                    Collection<string> resolvedPaths;
                    ProviderInfo provider;

                    try
                    {
                        resolvedPaths = SessionState.Path.GetResolvedProviderPathFromPSPath(path, out provider);
                    }
                    catch (System.Management.Automation.DriveNotFoundException e)
                    {
                        var er = new ErrorRecord(e, "DriveNotFound", ErrorCategory.ObjectNotFound, path);
                        WriteError(er);
                        continue;
                    }
                    catch (ItemNotFoundException e)
                    {
                        var er = new ErrorRecord(e, "PathNotFound", ErrorCategory.ObjectNotFound, path);
                        WriteError(er);
                        continue;
                    }

                    foreach (var resolved in resolvedPaths)
                    {
                        if (ValidateFile(resolved, provider))
                        {
                            filePaths.Add(resolved);
                        }
                    }
                }
            }
            else
            {
                foreach (var path in paths)
                {
                    string filePath;
                    ProviderInfo provider;
                    PSDriveInfo drive;

                    try
                    {
                        filePath = SessionState.Path.GetUnresolvedProviderPathFromPSPath(path, out provider, out drive);
                    }
                    catch (System.Management.Automation.DriveNotFoundException e)
                    {
                        var er = new ErrorRecord(e, "DriveNotFound", ErrorCategory.ObjectNotFound, path);
                        WriteError(er);
                        continue;
                    }

                    if (ValidateFile(filePath, provider))
                    {
                        filePaths.Add(filePath);
                    }
                }
            }

            return filePaths;
        }

        /// <summary>
        /// Filters package providers by the source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="providers">The providers.</param>
        protected IEnumerable<PackageProvider> FilterSource(string source, IEnumerable<PackageProvider> providers)
        {
            var instances = providers.Where(x => x.IsSource(source)).ToList();

            if (instances.Count == 0)
            {
                string message;
                if (MyInvocation.BoundParameters.ContainsKey(nameof(Provider)))
                {
                    message = $"Package provider '{Provider}' does not support '{source}' source.";
                }
                else
                {
                    message = $"No package providers support '{source}' source.";
                }

                throw new InvalidOperationException(message);
            }

            return instances;
        }

        /// <summary>
        /// Validates that the path is a file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="provider">The PS provider.</param>
        protected bool ValidateFile(string path, ProviderInfo provider)
        {
            if (provider.Name != "FileSystem")
            {
                var ex = new InvalidOperationException($"Path '{path}' is not a file system path.");
                var er = new ErrorRecord(ex, "PathNotFileSystemProvider", ErrorCategory.InvalidArgument, path);
                WriteError(er);
                return false;
            }

            if (!File.Exists(path))
            {
                var ex = new InvalidOperationException($"Path '{path}' is not a file.");
                var er = new ErrorRecord(ex, "PathNotFile", ErrorCategory.InvalidArgument, path);
                WriteError(er);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates that the package provider supports the operation.
        /// </summary>
        /// <param name="package">Package to validate.</param>
        /// <param name="operation">Operation to validate.</param>
        protected bool ValidateOperation(PackageInfo package, PackageProviderOperations operation)
        {
            if (package.Provider.Operations.HasFlag(operation))
            {
                return true;
            }
            else
            {
                var ex = new InvalidOperationException($"Package provider '{package.Provider.Name}' does not support this operation.");
                var er = new ErrorRecord(ex, "PackageProviderOperationNotSupported", ErrorCategory.InvalidOperation, Request.Name);
                WriteError(er);
                return false;
            }
        }

        /// <summary>
        /// Invokes the package operation on multiple package instances.
        /// </summary>
        /// <param name="package">The package label.</param>
        /// <param name="verb">The package operation.</param>
        /// <param name="instances">The package instance and operation.</param>
        /// <param name="shouldProcess">If should call ShouldProcess.</param>
        /// <param name="first">If should only process first provider.</param>
        protected void Invoke(string package,
                              string verb,
                              IDictionary<PackageProvider, InvokePackage> instances,
                              bool shouldProcess = false,
                              bool first = false)
        {
            if (instances.Count == 0)
            {
                return;
            }

            if (shouldProcess && !ShouldProcess(package))
            {
                return;
            }
            
            WriteVerbose($"{verb} '{package}' package.");

            foreach (var instance in instances.Keys)
            {
                InvokePackage invoke = instances[instance];
                Invoke(package, instance, invoke);

                if (first && Request.HasWriteObject)
                {
                    break;
                }
            }

            if (!Request.HasWriteObject && !WildcardPattern.ContainsWildcardCharacters(Request.Name))
            {
                var ex = new PackageNotFoundException(package);
                var er = new ErrorRecord(ex, "PackageNotFound", ErrorCategory.ObjectNotFound, package);
                WriteError(er);
            }
        }

        private void Invoke(string package, PackageProvider instance, InvokePackage operation)
        {
            WriteVerbose($"Calling '{instance.ProviderInfo.Name}' provider.");
            Request.ProviderInfo = instance.ProviderInfo;

            try
            {
                operation(Request);
            }
            catch (PipelineStoppedException)
            {
                throw;
            }
            catch (Exception e)
            {
                var ex = new PackageProviderException(e.Message, e);
                var er = new ErrorRecord(ex, "PackageProviderError", ErrorCategory.NotSpecified, package);
                WriteError(er);
            }
        }
    }
}
