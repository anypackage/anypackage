
# AnyPackage

![PowerShell Gallery](https://img.shields.io/powershellgallery/dt/AnyPackage?logo=powershell)
![NuGet](https://img.shields.io/nuget/dt/AnyPackage?logo=nuget)
![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/anypackage/anypackage/ci.yml)
![CodeFactor Grade](https://img.shields.io/codefactor/grade/github/anypackage/anypackage)

`AnyPackage` is a cross-platform PowerShell unified package management interface.
Manage multiple package managers from a single set of commands.

This project is a spiritual successor to `PackageManagement` also known as `OneGet`.
`AnyPackage` is not a fork of `PackageManagement` as such does not support any `PackageManagement` providers.

For more information on what `AnyPackage` is and how to use it refer to [about_AnyPackage](https://anypackage.dev/docs/reference/about_AnyPackage).

## Features

- PowerShell 5.1+
- Cross-platform Windows, MacOS, Linux
- Simple PowerShell cmdlets
- Manage package lifecycle
- Manage package sources
- PowerShell or C# package providers
- Simple and concise package provider authoring experience
- Package provider dynamic parameters
- Updatable help
- Argument completers
- DSC resources

## Install AnyPackage

```powershell
# PowerShellGet
Install-Module AnyPackage

# PSResourceGet
Install-PSResource AnyPackage

```

## Package Providers

`AnyPackage` uses a provider model.
Each package manager creates a package provider which implements the `AnyPackage` interface.
Package providers are shipped in a PowerShell module. For more information on how to use a provider refer to [about_Package_Providers](https://anypackage.dev/docs/reference/about_Package_Providers).

### Provider Catalog

To view the available package providers go to the [Provider Catalog](https://anypackage.dev/docs/provider-catalog).

### Creating a Provider

To learn how to create a package provider refer to [about_Creating_Package_Providers](https://anypackage.dev/docs/reference/about_Creating_Package_Providers).

## Documentation

Documentation is located on the [anypackage.dev](https://anypackage.dev) website.
The source is located in the [docs](https://github.com/anypackage/docs) repository.
