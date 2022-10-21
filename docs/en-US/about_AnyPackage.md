# AnyPackage

## about_AnyPackage

## Short Description

Describes what the `AnyPackage` module is and how to use it.

## Long Description

The `AnyPackage` is not a package management system in the traditional sense but rather a way to interact with multiple package management systems.
This lets users have a single set of commands to interact with any package management system instead of learning a unique set of commands.

## Package Providers

A package provider is the method used for `AnyPackage` to interface with a specific package management system.
For more information see [about_Package_Providers](about_Package_Providers.md).

## Creating Package Providers

To create your own package provider see [about_Creating_Package_Providers](about_Creating_Package_Providers.md).

## Finding Package Providers

Providers are shipped via PowerShell modules.
These modules can be identified by having the `AnyPackage` and `Provider` tags.

## Importing Package Providers

To import a package provider you will need to import the PowerShell module containing the provider. For more information see [about_Package_Providers](about_Package_Providers.md#importing-a-package-provider).

## Removing Package Providers

To remove a package provider you will need to remove the PowerShell module containing the provider.
For more information see [about_Package_Providers](about_Package_Providers.md#removing-a-package-provider).

## See Also

* [about_Package_Providers](about_Package_Providers.md)
* [about_Creating_Package_Providers](about_Creating_Package_Providers.md)
