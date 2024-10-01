// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Collections.ObjectModel;
using System.Management.Automation;

using AnyPackage.Provider;
using AnyPackage.Resources;

namespace AnyPackage.Commands.Internal;

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
            _request ??= new PackageRequest(this);
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
        Request.Version = package.Version is not null ? PackageVersionRange.Parse($"[{package.Version}]") : null;
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
    protected IEnumerable<PackageProvider> GetNameInstances()
    {
        var instances = GetInstances(Provider).Where(x => x.ProviderInfo.PackageByName).ToList();

        if (instances.Count == 0)
        {
            string message;
            if (MyInvocation.BoundParameters.ContainsKey(nameof(Provider)))
            {
                message = string.Format(Strings.PackageProviderNotSupportPackageByName, Provider);
            }
            else
            {
                message = Strings.NoPackageProvidersSupportPackageByName;
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
                message = string.Format(Strings.PackageProviderExtensionNotSupported, Provider, extension);
            }
            else
            {
                message = string.Format(Strings.NoPackageProviderSupportsExtension, extension);
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
                message = string.Format(Strings.PackageProviderUriSchemeNotSupported, Provider, uri.Scheme);
            }
            else
            {
                message = string.Format(Strings.NoPackageProviderSupportsUriScheme, Provider, uri.Scheme);
            }

            var ex = new InvalidOperationException(message);
            var er = new ErrorRecord(ex, "PackageProviderUriSchemeNotSupported", ErrorCategory.InvalidOperation, uri);
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
                try
                {
                    var filePath = SessionState.Path.GetUnresolvedProviderPathFromPSPath(path, out var provider, out var drive);

                    if (ValidateFile(filePath, provider))
                    {
                        filePaths.Add(filePath);
                    }
                }
                catch (System.Management.Automation.DriveNotFoundException e)
                {
                    var er = new ErrorRecord(e, "DriveNotFound", ErrorCategory.ObjectNotFound, path);
                    WriteError(er);
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
                message = string.Format(Strings.PackageProviderSourceNotSupported, Provider, source);
            }
            else
            {
                message = string.Format(Strings.NoPackageProviderSupportsSource, source);
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
            var ex = new InvalidOperationException(string.Format(Strings.PathNotFileSystemProvider, path));
            var er = new ErrorRecord(ex, "PathNotFileSystemProvider", ErrorCategory.InvalidArgument, path);
            WriteError(er);
            return false;
        }

        if (!File.Exists(path))
        {
            var ex = new InvalidOperationException(string.Format(Strings.PathNotFile, path));
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
            var ex = new InvalidOperationException(string.Format(Strings.PackageProviderOperationNotSupported, package.Provider.Name));
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

        WriteVerbose(string.Format(Strings.OperationPackage, verb, package));

        var orderedInstances = instances.Keys.OrderBy(x => x.ProviderInfo.Priority)
                                             .ThenBy(x => x.ProviderInfo.FullName);

        foreach (var instance in orderedInstances)
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
        WriteVerbose(string.Format(Strings.CallingProvider, instance.ProviderInfo.Name));
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
            var er = new ErrorRecord(e, "PackageProviderError", ErrorCategory.NotSpecified, package);
            WriteError(er);
        }
    }
}
