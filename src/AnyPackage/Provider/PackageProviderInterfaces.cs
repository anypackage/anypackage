// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

namespace AnyPackage.Provider
{
    /// <summary>
    /// Interface to support <c>Find-Package</c> command.
    /// </summary>
    public interface IFindPackage
    {
        void FindPackage(PackageRequest request);
    }

    /// <summary>
    /// Interface to support <c>Get-Package</c> command.
    /// </summary>
    public interface IGetPackage
    {
        void GetPackage(PackageRequest request);
    }

    /// <summary>
    /// Interface to support <c>Install-Package</c> command.
    /// </summary>
    public interface IInstallPackage
    {
        void InstallPackage(PackageRequest request);
    }

    /// <summary>
    /// Interface to support <c>Publish-Package</c> command.
    /// </summary>
    public interface IPublishPackage
    {
        void PublishPackage(PackageRequest request);
    }

    /// <summary>
    /// Interface to support <c>Save-Package</c> command.
    /// </summary>
    public interface ISavePackage
    {
        void SavePackage(PackageRequest request);
    }

    /// <summary>
    /// Interface to support <c>Uninstall-Package</c> command.
    /// </summary>
    public interface IUninstallPackage
    {
        void UninstallPackage(PackageRequest request);
    }

    /// <summary>
    /// Interface to support <c>Update-Package</c> command.
    /// </summary>
    public interface IUpdatePackage
    {
        void UpdatePackage(PackageRequest request);
    }

    /// <summary>
    /// Interface to support <c>Get-PackageSource</c> command.
    /// </summary>
    public interface IGetSource
    {
        void GetSource(SourceRequest request);
    }

    /// <summary>
    /// Interface to support user defined repositories and commands:
    /// <c>Set-PackageSource</c>, <c>Register-PackageSource</c>,
    /// <c>Unregister-PackageSource</c> command.
    /// </summary>
    public interface ISetSource
    {
        void RegisterSource(SourceRequest request);
        void SetSource(SourceRequest request);
        void UnregisterSource(SourceRequest request);
    }
}
