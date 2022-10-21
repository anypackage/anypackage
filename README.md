# UniversalPackageManager

`PackageManagement` is being retired by Microsoft.
The role it played is still needed by the community.
`PackageManagement` was unable deliver the promise of a universal package manager due to the lack of package providers.

`UniversalPackageManager` (UPM) is a spiritual successor to `PackageManagement`.
The project will make use of the lessons learned and use design principles from `PowerShellGet` v3.

For more information on what `UniversalPackageManager` is and how to use it refer to [about_UniversalPackageManager](docs/en-US/about_UniversalPackageManager.md).

## PowerShellGet

`UPM` will support managing packages from `PowerShellGet` v3.
The goal of this project is to be a superset of `PowerShellGet` v3 by supporting `PowerShellGet` package types and other package management systems. For more information about `PowerShellGet` v3 refer to [PowerShell/PowerShell-RFC#185](https://github.com/PowerShell/PowerShell-RFC/pull/185).

## PackageManagement

This project will not be compatible with `PackageManagement` or existing package providers.

## Motivation

```none
As a PowerShell user,
I want to learn a single set of cmdlets to manage multiple package managers,
so I can be more productive.
```

## Package Providers

`UPM` uses a provider model.
Each package management system creates a package provider which implements the `UPM` interface.
Package providers are shipped in a PowerShell module. For more information on how to use a provider refer to [about_Package_Providers](docs/en-US/about_Package_Providers).

### Creating a Provider

To learn how to create a package provider refer to [about_Creating_Package_Providers](docs/en-US/about_Creating_Package_Providers.md).

## Core Providers

`UPM` will implement the following providers to match the current scope of `PackageManagement`.
The providers will not be shipped with `UPM` and will separate modules to better handle updating.

- [PowerShellGet v3](https://github.com/ThomasNieto/UniversalPackageManager.Provider.PowerShellGet)
- MSI
- MSU
- Programs
- NuGet
