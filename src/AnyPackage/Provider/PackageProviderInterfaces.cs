// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

namespace AnyPackage.Provider;

/// <summary>
/// Interface to support <c>Find-Package</c> command.
/// </summary>
public interface IFindPackage
{
    /// <summary>
    /// Finds packages with the specified request.
    /// </summary>
    /// <remarks>
    /// If the requested package is not found, no exception should be thrown.
    /// </remarks>
    /// <param name="request">Package request.</param>
    void FindPackage(PackageRequest request);
}

/// <summary>
/// Interface to support <c>Get-Package</c> command.
/// </summary>
public interface IGetPackage
{
    /// <summary>
    /// Gets packages with the specified request.
    /// </summary>
    /// <remarks>
    /// If the requested package is not found, no exception should be thrown.
    /// </remarks>
    /// <param name="request">Package request.</param>
    void GetPackage(PackageRequest request);
}

/// <summary>
/// Interface to support <c>Install-Package</c> command.
/// </summary>
public interface IInstallPackage
{
    /// <summary>
    /// Installs packages with the specified request.
    /// </summary>
    /// <remarks>
    /// If the requested package is not found, no exception should be thrown.
    /// </remarks>
    /// <param name="request">Package request.</param>
    void InstallPackage(PackageRequest request);
}

/// <summary>
/// Interface to support <c>Publish-Package</c> command.
/// </summary>
public interface IPublishPackage
{
    /// <summary>
    /// Publishes packages with the specified request.
    /// </summary>
    /// <remarks>
    /// If the requested package is not found, no exception should be thrown.
    /// </remarks>
    /// <param name="request">Package request.</param>
    void PublishPackage(PackageRequest request);
}

/// <summary>
/// Interface to support <c>Save-Package</c> command.
/// </summary>
public interface ISavePackage
{
    /// <summary>
    /// Saves packages with the specified request.
    /// </summary>
    /// <remarks>
    /// If the requested package is not found, no exception should be thrown.
    /// </remarks>
    /// <param name="request">Package request.</param>
    void SavePackage(PackageRequest request);
}

/// <summary>
/// Interface to support <c>Uninstall-Package</c> command.
/// </summary>
public interface IUninstallPackage
{
    /// <summary>
    /// Uninstalls packages with the specified request.
    /// </summary>
    /// <remarks>
    /// If the requested package is not found, no exception should be thrown.
    /// </remarks>
    /// <param name="request">Package request.</param>
    void UninstallPackage(PackageRequest request);
}

/// <summary>
/// Interface to support <c>Update-Package</c> command.
/// </summary>
public interface IUpdatePackage
{
    /// <summary>
    /// Updates packages with the specified request.
    /// </summary>
    /// <remarks>
    /// If the requested package is not found, no exception should be thrown.
    /// </remarks>
    /// <param name="request">Package request.</param>
    void UpdatePackage(PackageRequest request);
}

/// <summary>
/// Interface to support <c>Get-PackageSource</c> command.
/// </summary>
public interface IGetSource
{
    /// <summary>
    /// Gets package sources with the specified request.
    /// </summary>
    /// <remarks>
    /// If the requested source is not found, no exception should be thrown.
    /// </remarks>
    /// <param name="request">Package request.</param>
    void GetSource(SourceRequest request);
}

/// <summary>
/// Interface to support user defined repositories and commands:
/// <c>Set-PackageSource</c>, <c>Register-PackageSource</c>,
/// <c>Unregister-PackageSource</c> command.
/// </summary>
public interface ISetSource
{
    /// <summary>
    /// Gets package sources with the specified request.
    /// </summary>
    /// <param name="request">Package request.</param>
    void RegisterSource(SourceRequest request);

    /// <summary>
    /// Sets package sources with the specified request.
    /// </summary>
    /// <remarks>
    /// If the requested source is not found, no exception should be thrown.
    /// </remarks>
    /// <param name="request">Package request.</param>
    void SetSource(SourceRequest request);

    /// <summary>
    /// Unregister a package source with the specified request.
    /// </summary>
    /// <remarks>
    /// If the requested source is not found, no exception should be thrown.
    /// </remarks>
    /// <param name="request">Package request.</param>
    void UnregisterSource(SourceRequest request);
}
