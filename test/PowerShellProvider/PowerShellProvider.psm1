using module AnyPackage

using namespace AnyPackage.Provider
using namespace System.Collections.Generic

[PackageProvider('PowerShell')]
class PowerShellProvider : PackageProvider, IFindPackage, IGetPackage, IGetSource {
    PowerShellProvider() : base('89d76409-f1b0-46cb-a881-b012be54aef5') { }

    [PackageProviderInfo] Initialize([PackageProviderInfo] $providerInfo) {
        return [PowerShellProviderInfo]::new($providerInfo)
    }

    [void] FindPackage([PackageRequest] $request) {
        if ($request.Source) {
            $sources = $this.ProviderInfo.Sources |
            Where-Object Name -eq $request.Source
        }
        else {
            $sources = $this.ProviderInfo.Sources
        }

        foreach ($source in $sources) {
            $path = Join-Path -Path $source.Location -ChildPath *.json
            Get-ChildItem -Path $path |
            ForEach-Object {
                Get-Content -Path $_.FullName |
                ConvertFrom-Json |
                Where-Object { $request.IsMatch($_.Name, $_.Version) } |
                Write-Package -Request $request -Source $source
            }
        }
    }

    [void] GetPackage([PackageRequest] $request) {
        $this.ProviderInfo.Packages |
        Where-Object { $request.IsMatch($_.Name, $_.version) } |
        Write-Package -Request $request
    }

    [void] GetSource([SourceRequest] $sourceRequest) {
        $this.ProviderInfo.Sources |
        Where-Object Name -like $sourceRequest.Name |
        Write-Source -SourceRequest $sourceRequest
    }
}

class PowerShellProviderInfo : PackageProviderInfo {
    # Installed packages
    [List[object]] $Packages = [List[object]]::new()

    # Registered sources
    [List[object]] $Sources = [List[object]]::new()

    PowerShellProviderInfo([PackageProviderInfo] $providerInfo) : base($providerInfo) {
        $this.Sources += @([PSCustomObject]@{
            Name = 'Default'
            Location = (Resolve-Path -Path (Join-Path -Path $PSScriptRoot -ChildPath "../packages")).Path
            Trusted = $true
        })
    }
}

[PackageProviderManager]::RegisterProvider([PowerShellProvider], $MyInvocation.MyCommand.ScriptBlock.Module)

$MyInvocation.MyCommand.ScriptBlock.Module.OnRemove = {
    [PackageProviderManager]::UnregisterProvider([PowerShellProvider])
}

function Write-Source {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory,
            ValueFromPipeline)]
        [object]
        $InputObject,

        [Parameter(Mandatory)]
        [SourceRequest]
        $SourceRequest
    )

    process {
        $SourceRequest.WriteSource($InputObject.Name,
                                   $InputObject.Location,
                                   $InputObject.Trusted,
                                   $InputObject.Metadata)
    }
}

function Write-Package {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory,
            ValueFromPipeline)]
        [object]
        $InputObject,

        [Parameter(Mandatory)]
        [PackageRequest]
        $Request,

        [Parameter()]
        [object]
        $Source
    )

    process {
        if ($Source) {
            $Source = $Request.NewSourceInfo($Source.Name, $Source.Location, $Source.Trusted)
        }

        $Request.WritePackage($InputObject.Name,
                              $InputObject.Version,
                              $InputObject.Description,
                              $Source,
                              $InputObject.Metadata)
    }
}
