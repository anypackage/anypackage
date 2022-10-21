# Creating_Package_Providers

## about_Creating_Package_Providers

## Short Description

Describes how to create a package provider.

## Long Description

A package provider is the way for module authors to extend the `UniversalPackageManager` module.
Providers can be created in PowerShell or C#.
Package providers are implemented by defining a class that inherits from `PackageProvider` class and has the `PackageProvider` attribute.

## PackageProvider Attribute

The `PackageProvider` attribute defines the package provider name.
Additional configuration may be added in the future to define optional features.

```powershell
[PackageProvider("ProviderName")]
```

## PackageProvider Class

The `PackageProvider` base class serves as the foundation for all package providers.

### Constructing

The package provider must have a public parameter-less constructor that calls the base constructor with the package provider's unique identifier `[Guid]`.

```powershell
[PackageProvider('Test')]
class TestProvider : PackageProvider {
    TestProvider() : base('e5491948-72b3-4f00-aa64-f93060d9b242') { }
}
```

### Initializing

`UniversalPackageManager` creates a new instance of the `PackageProvider` each time a cmdlet is called.
This makes the package provider stateless.
If a package provider requires one-time initialization override the `Initialize` method.

If your provider needs to maintain state between instances then you can create a class that inherits from `PackageProviderInfo` to store state or user accessible information.
The derived `PackageProviderInfo` will be sent to each instance of the package provider.

```powershell
[PackageProvider('Test')]
class TestProvider : PackageProvider {
    [PackageProviderInfo] Initialize([PackageProviderInfo] $providerInfo) {
        return [MyProviderInfo]::new()
    }
}

class MyProviderInfo : PackageProviderInfo {
    [SqlConnection] $Connection
}
```

### Uninitializing

To perform one-time provider clean-up to free up any resources or connections override the `Clean` method.

```powershell
[PackageProvider('Test')]
class TestProvider : PackageProvider {
    [void] Clean() {
        # Clean-up logic 
    }
}
```

### Dynamic Parameters

To add provider specific parameters for a command override the `GetDynamicParameters` method.
The `$commandName` parameter will be one of the cmdlets such as `Get-Package`.
The method can return `$null`, a `[RuntimeDefinedParameterDictionary]` object or an object with properties that have `[Parameter()]` attribute.

Defining a class with properties is the easiest way to create dynamic parameters.
The syntax is very similar to the `param()` block in a PowerShell function.
There are few notable differences, one being that each property must have a `[Parameter()]` attribute in order for the PowerShell runtime to treat it as a parameter.
Secondly there is no comma after each parameter.

```powershell
[PackageProvider('Test')]
class TestProvider : PackageProvider {
    [object] GetDynamicParameters([string] $commandName) {
        if ($commandName -eq 'Get-Package') {
            return [GetPackageDynamicParameters]::new()
        } else {
            return $null
        }
    }
}

class GetPackageDynamicParameters {
    [Parameter()]
    [string] $Path

    [Parameter()]
    [ScopeType] $Scope
}
```

### Supporting Operations

The package provider indicates support for each individual `UniversalPackageManager` cmdlet by adding a corresponding interface.
For example, if the package provider supports `Get-Package` cmdlet then the `IGetPackage` interface would be implemented.

| Cmdlet                       | Interface         |
| ------                       | ---------         |
| Find-Package                 | IFindPackage      |
| Get-Package                  | IGetPackage       |
| Install-Package              | IInstallPackage   |
| Publish-Package              | IPublishPackage   |
| Save-Package                 | ISavePackage      |
| Update-Package               | IUpdatePackage    |
| Uninstall-Package            | IUninstallPackage |
| Get-PackageSource            | IGetSource        |
| Set-PackageSource            | ISetSource        |
| Register-PackageSource       | ISetSource        |
| Unregister-PackageSource     | ISetSource        |

If a `Package` method was successful `WritePackage` must be called with the package details.
In the event a package is not found by the provider do not throw an exception as `UniversalPackageManager` will write an error.

The package interfaces follow the structure as follows.

```powershell
[PackageProvider('Test')]
class TestProvider : PackageProvider, IGetPackage {
    [void] GetPackage([PackageRequest] $request) { }
}
```

If a `Source` method was successful `WriteSource` must be called with the source details.
In the event a source is not found by the provider do not throw an exception as `UniversalPackageManager` will write an error.

The package source interfaces follow the structure as follows.

```powershell
[PackageProvider('Test')]
class TestProvider : PackageProvider, IGetSource {
    [void] GetPackageSource([SourceRequest] $request) { }
}
```

The `ISetSource` interface is different as it requires three methods compared to the rest.

```powershell
[PackageProvider('Test')]
class TestProvider : PackageProvider, ISetSource {
    [void] SetPackageSource([SourceRequest] $request) { }

    [void] RegisterPackageSource([SourceRequest] $request) { }

    [void] UnregisterPackageSource([SourceRequest] $request) { }
}
```

### Package Request

The `[PackageRequest]` type contains information about the request and methods to interact with `UniversalPackageManager`.

```powershell
class PackageRequest {
    [string] $Name
    [VersionRange] $Version
    [string] $Source
    [bool] $Prerelease
    [PackageInfo] $Package
    [string] $Path
    [object] $DynamicParameters

    [bool] $Stopping

    [bool] IsMatch([string] $name)
    [bool] IsMatch([NuGetVersion] $version)
    [bool] IsMatch([string] $name, [NuGetVersion] $version)

    [bool] PromptUntrustedSource([string] $source)

    [void] WritePackage([PackageInfo] $package)
    [void] WritePackage([IEnumerable[PackageInfo]] $package)
    [void] WritePackage([string] $name, [NuGetVersion] $version, [string] $description, [PackageSourceInfo] $source, [hashtable] $metadata, [IEnumerable[PackageDependency]] $dependencies)

    [PackageSourceInfo] NewSourceInfo([string] $name, [string] $location, [bool] $trusted, [hashtable] $metadata)
```

### Source Request

The `[SourceRequest]` type contains information about the request and methods to interact with `UniversalPackageManager`.

```powershell
class SourceRequest {
    [string] $Name
    [string] $Location
    [bool] $Trusted
    [bool] $Force
    [object] $DynamicParameters

    [bool] $Stopping

    [void] WriteSource([PackageSourceInfo] $source)
    [void] WriteSource([IEnumerable[PackageSourceInfo]] $source)
    [void] WriteSource([string] $name, [string] $location, [bool] $trusted, [hashtable] $metadata)
```

## Register a Package Provider

To register a package provider with `UniversalPackageManager` the following method must be called from within your module.
In this example the `[TestProvider]` is the type that implements the package provider.

```powershell
[PackageProviderManager]::RegisterProvider([TestProvider], $MyInvocation.MyCommand.ScriptBlock.Module)
```

If you are unable to pass the `PSModuleInfo` then the module name can be passed instead.

```powershell
[PackageProviderManager]::RegisterProvider([TestProvider], 'TestModule')
```

## Unregister a Package Provider

To remove a package provider on when the provider module is removed add this command.
In this example the `[TestProvider]` is the type that implements the package provider.

```powershell
$MyInvocation.MyCommand.ScriptBlock.Module.OnRemove = { 
    [PackageProviderManager]::UnregisterProvider([TestProvider])
}
```

## Examples

### Basic Provider

The following example is the minimum code required to define a package provider.

```powershell
using module UniversalPackageManager
using namespace UniversalPackageManager.Provider

[PackageProvider('Test')]
class TestProvider : PackageProvider {
    TestProvider() : base('e5491948-72b3-4f00-aa64-f93060d9b242') { }
}

[PackageProviderManager]::RegisterProvider([TestProvider], $MyInvocation.MyCommand.ScriptBlock.Module)

$MyInvocation.MyCommand.ScriptBlock.Module.OnRemove = { 
    [PackageProviderManager]::UnregisterProvider([TestProvider])
}
```

## Documenting

The package provider should come with an about topic to describe the provider any capabilities it has and any dynamic parameters.

## See Also

* [about_Package_Providers](about_Package_Providers.md)
* [about_UniversalPackageManager](about_UniversalPackageManager.md)
