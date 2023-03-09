# AnyPackage

`AnyPackage` is a spiritual successor to `PackageManagement` also known as `OneGet`.

`AnyPackage` provides the ability to use multiple package management systems through a single set of commands.

For more information on what `AnyPackage` is and how to use it refer to [about_AnyPackage](https://anypackage.dev/docs/reference/about_AnyPackage).

## PackageManagement

`AnyPackage` is not a fork of `PackageManagement` as such does not support any `PackageManagement` providers.

## Package Providers

`AnyPackage` uses a provider model.
Each package management system creates a package provider which implements the `AnyPackage` interface.
Package providers are shipped in a PowerShell module. For more information on how to use a provider refer to [about_Package_Providers](https://anypackage.dev/docs/reference/about_Package_Providers).

### Provider Catalog

To view the available package providers go to the [Provider Catalog](https://anypackage.dev/docs/provider-catalog).

### Creating a Provider

To learn how to create a package provider refer to [about_Creating_Package_Providers](https://anypackage.dev/docs/reference/about_Creating_Package_Providers).

## Documentation

Documentation is located on the [anypackage.dev](https://anypackage.dev) website.
The source is located in the [AnyPackage-Docs](https://github.com/AnyPackage/AnyPackage-Docs) repository.
