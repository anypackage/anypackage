# UniversalPackageManager

`PackageManagement` is being retired by Microsoft.
The role it played is still needed by the community.
`PackageManagement` was unable deliver the promise of a universal package manager due to the lack of package providers.

`UniversalPackageManager` (UPM) is a spiritual successor to `PackageManagement`.
The project will make use of the lessons learned and use design principles from `PowerShellGet` v3.

## PowerShellGet

`UPM` will support managing packages from `PowerShellGet` v3.
The goal of this project is to be a superset of `PowerShellGet` v3 by supporting `PowerShellGet` package types and other package management systems. For more information about `PowerShellGet` v3 refer to [PowerShell/PowerShell-RFC#185](https://github.com/PowerShell/PowerShell-RFC/pull/185).

## Package Management

This project will not be compatible with `PackageManagement` or existing package providers.

## Motivation

```none
As a PowerShell user,
I want to learn a single set of cmdlets to manage multiple package managers,
so I can be more productive.
```

## Package Providers

`UPM` will use a provider model.
Each package management system will create a package provider which implements a `UPM` API.
Package providers will be shipped from PowerShell modules.

Provider's `psm1` will be imported into a separate runspace.
This allows for multi-threaded workflows.
Runspaces will be removed when `UPM` is removed.

## Specification

### Provider Discovery

To support finding package providers without downloading and parsing `psd1` the use of `UPMProvider_<Name>` for example `UPMProvider_NuGet` can be used.

The `UPMProviders` hashtable consists of a `Name` and `Path` pair.
This is used for `UPM` to know the provider name and `psm1` path without having to import the `psm1`.
This method also allows for a single module shipping multiple package providers.

```powershell
@{
    PrivateData = @{
        UPMProviders = @{
            Name = 'NuGet'
            Path = 'NuGet.psm1'
        }

        PSData = @{
            Tags = @('UniversalPackageManager', 'Provider', 'UPMProvider_NuGet')
        }
    }
}
```

```powershell
@{
    PrivateData = @{
        UPMProviders = @(
            @{
                Name = 'NuGet'
                Path = 'NuGet.psm1'
            },
            @{
                Name = 'Chocolatey'
                Path = 'Chocolatey.psm1'
            }
        )

        PSData = @{
            Tags = @('UniversalPackageManager', 'Provider', 'UPMProvider_NuGet', 'UPMProvider_Chocolatey')
        }
    }
}
```

### API

The API will be a collection of PowerShell functions.
If a package provider does not support a specific API `UPM` will output an invalid operation error.
The name of the API function will match the name of the `UPM` cmdlet.
In the event the provider needs to make a call to `UPM` for another provider or internally the fully qualified cmdlet name will be needed.
For example `UniversalPackageManager\Find-Package`.

### Provider Configuration/Features

To indicate which package providers support different features a hashtable will be used.
If a feature is not declared the system default to not supported.

```powershell
@{
    FindPackageBeforeInstall  = $true
    GetPackageBeforeUninstall = $true
    FindPackageBeforeUpdate   = $true
    GetPackageBeforeUpdate    = $true
    SkipUpdateSameVersion     = $true
    Wildcard                  = $true
    Credential                = $true
    DestinationPath           = $true
    FileExtensions            = @('zip')
    FileSignatures            = @('50 4B 03 04')
    UriSchemes                = @('http', 'https', 'file')
    VersionScheme             = [VersionScheme]::Semver
}
```

- `FindPackageBeforeInstall`: `Install-Package` implicitly calls `Find-Package` when not using `InputObject` pipeline input.

- `GetPackageBeforeUninstall`: `Uninstall-Package` implicitly calls `Get-Package` when not using `InputObject` pipeline input.

- `FindPackageBeforeUpdate`: `Update-Package` implicitly calls `Find-Package` when not using `InputObject` pipeline input.

- `GetPackageBeforeUpdate`: `Update-Package` implicitly calls `Get-Package` and errors if package is not found.

- `SkipUpdateSameVersion`: Only valid if `FindPackageBeforeUpdate` and `GetPackageBeforeUpdate` is true.
`Update-Package` will skip the update if `Find-Package` and `Get-Package` return the same version.
Disable this when supporting reinstalling with `Update-Package`.

- `Wildcard`: Supports wildcard characters on `Name` parameter.

- `Credential`: Supports `Credential` parameter.

- `DestinationPath`: Supports `DestinationPath` parameter.

- `FileExtensions`: Supported file extensions.

- `FileSignatures`: Supported file signatures at the beginning of files.
This is used to determine the type of file when file extensions are not present.
For more information [File Signatures](https://en.wikipedia.org/wiki/List_of_file_signatures).

- `VersionScheme`: [ISO 19770-2](https://standards.iso.org/iso/19770/-2/2015/schema.xsd) version schemes used for packages.

| Scheme | Enum | Example | Description |
| --- | --- | --- | --- |
| alphanumeric | AlphaNumeric | beta1 | Strictly a string, sorting alphanumerically |
| decimal | 1.25 | Decimal | A floating point number |
| multipartnumeric | MultiPartNumeric | 1.0.0.0 | Numbers separated by dots, where the numbers are interpreted as integers |
| multipartnumeric+suffix | MultiPartNumericSuffix | 1.2.3a | Numbers seperated by dots, where the numbers are interpreted as integers with an additional string suffix |
| semver | Semver | 1.0.0-beta1 | Follows the semver.org spec |
| unknown | Unknown | | No attempt should be made to order these |

### Objects

#### Provider

```powershell
class ProviderInfo {
    [string] $Name
    [ISO197702Version] $Version
    [string] $Path
    [hashtable] $Metadata
}
```

#### Source

```powershell
class SourceInfo {
    [string] $Name
    [string] $Location
    [string] $Provider
    [hashtable] $Metadata
    [bool] $IsRegistered
    [bool] $IsTrusted
    [bool] $IsValidated
}
```

#### Package

```powershell
class PackageInfo {
    [string] $Name
    [string] $Source
    [string] $Provider
    [hashtable] $Metadata
    [string] $Summary
    [string] $Description
    [ISO197702Version] $Version
    [DependencyInfo] $Dependencies

    [void] AddDependency([string] $Name, [string] $Version, [string] $Source) { }
    [void] AddDependency([string] $Name, [string] $Version, [string] $Source, [string] $Provider) { }
}
```

#### Dependency

Package providers will be able to add dependencies from other package providers.

```powershell
class DependencyInfo {
    [string] $Name
    [DependencyVersion] $Version
    [SourceInfo] $Source
}

class DependencyVersion {
    [string] $Version
    [string] $RequiredVersion
    [string] $MinimumVersion
    [string] $MaximumVersion
    [bool] $MinimumVersionExclusive
    [bool] $MaximumVersionExclusive

    [bool] TestVersion([string]$Version) { }

    [string] ToString() {
        $this.Version
    }
}
```

#### ISO 19770-2 Version Schemes

One major pitfall of using `PackageManagement` not being able to sort versions.
The `Version` property was of type `[string]` causing invalid sorting.
To avoid that `UPM` will implement an object using `ISO 1770-2` version schemes.
The object will implement the `System.IComparable` interface to facilitate sorting based on version scheme.

```powershell
enum VersionScheme {
    AlphaNumeric
    Decimal
    MultiPartNumeric
    MultiPartNumericSuffix
    Semver
    Unknown
}

class ISO197702Version : System.IComparable {
    [string] $Version
    [VersionScheme] $VersionScheme = [VersionScheme]::Unknown

    [int] CompareTo([ISO197702Version] $Version) { }
}
```

## Cmdlets

> Cmdlet prefix has not been finalized. The tentative value is `UPM`.

### Dynamic Parameters

Package providers need a way to expose parameters for `UPM` cmdlets.
To accomplish that task dynamic parameters will be used.
Dynamic parameters can have a discoverability issue leading to a poor user experience.
To alleviate this issue the recommended use case will be for users to first use `-Provider` parameter to indicate which package provider will be used.
Then `CTRL+SPACE` or `TAB` completion can be used to discover newly created parameters.
The package provider will also ship an `about_<provider>` help file documentation about how to use the provider and each dynamic parameter.

Each package provider will have a parameter set to define dynamic parameters using a helper function.
This will help with the complexity of creating dynamic parameters as well as enforce standards to avoid conflicts with other package providers.
The provider will be able to have different parameters for each cmdlet exposed by `UPM`.

### Provider Management

Package providers will not be imported automatically when the module is imported.
The specific provider will be imported when `-Provider` parameter is used.

- `Get-PackageProvider`
- `Import-PackageProvider`
- `Remove-PackageProvider`

May implement the following cmdlets if the need arises.

- `Find-PackageProvider`
- `Install-PackageProvider`
- `Uninstall-PackageProvider`
- `Update-PackageProvider`

| Name | Type | Pipeline | Description |
| --- | --- | --- | --- |
| Name | `[string]` | ByPropertyName | Provider name |
| Version | `[string]` | ByPropertyName | Provider version using NuGet version format |

### Package Sources

- `Get-PackageSource`
- `Register-PackageSource`
- `Unregister-PackageSource`
- `Set-PackageSource`

| Name | Type | Pipeline | Description |
| --- | --- | --- | --- |
| Name | `[string]` | ByPropertyName | Source name |
| Location | `[string]` | ByPropertyName | Source location be that URL, path, or something else |
| Trusted | `[switch]` | | If source is trusted |

### Package Life Cycle

- `Find-Package`
- `Install-Package`
- `Uninstall-Package`
- `Get-Package`
- `Update-Package`

| Name | Type | Pipeline | Description |
| --- | --- | --- | --- |
| Name | `[string]` | ByPropertyName | Package name |
| Source | `[string]` | ByPropertyName | Package source friendly name or location be that URL, path, or something else |
| Version | `[string]` | ByPropertyName | Provider version using [NuGet version ranges and wildcard format](https://docs.microsoft.com/en-us/nuget/reference/package-versioning#version-ranges-and-wildcards) |
| Trusted | `[switch]` | | Indicate source is trusted for this execution only |
| Credential | `[pscredential]` | ByPropertyName | Source credential |
| Proxy | `[uri]` | ByType, ByPropertyName | Proxy URI |
| ProxyCredential | `[pscredential]` | ByPropertyName | Proxy credential |
| RequiredPackages | `[string]` | ByValue, ByPropertyName | A hashtable or json using the [Required Packages Format](#Required-Packages-Format) |
| RequiredPackagesFile | `[string]` | ByValue, ByPropertyName | A path to a file with a hashtable or json using the [Required Packages Format](#Required-Packages-Format)  |

#### Required Packages Format

The required package format comes in two formats.
The first is the module name as the key and a NuGet version range and wildcard format value.
The second is the module name as the key and a hashtable containing parameters (splatting).

```powershell
@{
    PSReadline = '1.2.0.0'
    Configuration = @{
        Version = '[4.2.2,4.7.0]'
        Source  = 'https://www.powershellgallery.com'
        Trusted = $true
    }
    platyPS = @{
        Version    = '3.0.0'
        Source     = 'PSGallery'
        Provider   = 'PowerShellGet'
        Credential = $Credential
    }
}
```

### Authoring Packages

- `New-Package`
- `Publish-Package`
- `Unpublish-Package`

## Core Providers

`UPM` will implement the following providers to match the current scope of `PackageManagement`.
The providers will not be shipped with `UPM` and will separate modules to better handle updating.

- `PowerShellGet` v3
- `MSI`
- `MSU`
- `Programs`
- `NuGet`
