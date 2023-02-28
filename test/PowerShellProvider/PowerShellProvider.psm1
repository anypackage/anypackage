using module AnyPackage

using namespace AnyPackage.Provider
using namespace System.Collections.Generic

[PackageProvider('PowerShell')]
class PowerShellProvider : PackageProvider, IFindPackage, IGetPackage,
    IInstallPackage, IPublishPackage, ISavePackage, IGetSource, ISetSource {
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

    [void] InstallPackage([PackageRequest] $request) {
        $params = @{
            Name = $request.Name
            Prerelease = $request.Prerelease
            ErrorAction = 'Ignore'
        }

        if ($request.Version) {
            $params['Version'] = $request.Version
        }

        if ($request.Source) {
            $params['Source'] = $request.Source
        }
        
        Find-Package @params |
        Get-Latest |
        ForEach-Object {
            $this.ProviderInfo.Packages += $_
            $_ | Write-Package -Request $request -Source $_.Source
        }
    }

    [void] PublishPackage([PackageRequest] $request) {
        if (-not (Test-Path -Path $request.Path)) { return }

        $package = Get-Content -Path $request.Path |
        ConvertFrom-Json

        if ($request.Source) {
            $sourceName = $request.Source
        }
        else {
            $sourceName = 'Default'
        }

        $source = $this.ProviderInfo.Sources |
        Where-Object { $_.Name -eq $sourceName }

        Copy-Item -Path $request.Path -Destination $source.Location -ErrorAction Stop

        $package |
        Write-Package -Request $request -Source $source
    }

    [void] SavePackage([PackageRequest] $request) {
        $params = @{
            Name = $request.Name
            Prerelease = $request.Prerelease
            ErrorAction = 'Ignore'
        }

        if ($request.Version) {
            $params['Version'] = $request.Version
        }

        if ($request.Source) {
            $params['Source'] = $request.Source
        }
        
        Find-Package @params |
        Get-Latest |
        ForEach-Object {
            $path = Join-Path -Path $_.Source.Location -ChildPath "$($_.Name)-$($_.Version).json"
            Copy-Item -Path $path -Destination $request.Path

            $_ | Write-Package -Request $request -Source $_.Source
        }
    }

    [void] GetSource([SourceRequest] $sourceRequest) {
        $this.ProviderInfo.Sources |
        Where-Object Name -like $sourceRequest.Name |
        Write-Source -SourceRequest $sourceRequest
    }

    [void] RegisterSource([SourceRequest] $sourceRequest) {
        $source = [PSCustomObject]@{
            Name = $sourceRequest.Name
            Location = $sourceRequest.Location
            Trusted = $sourceRequest.Trusted
        }

        $this.ProviderInfo.Sources += $source

        $source |
        Write-Source -SourceRequest $sourceRequest
    }

    [void] SetSource([SourceRequest] $sourceRequest) {
        
    }

    [void] UnregisterSource([SourceRequest] $sourceRequest) {
        
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

function Get-Latest {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory,
            ValueFromPipeline)]
        [object]
        $Package
    )

    begin {
        $packages = [List[object]]::new()
    }

    process {
        $packages.Add($Package)
    }

    end {
        $packages |
        Group-Object -Property Name |
        ForEach-Object {
            $_.Group |
            Sort-Object -Property Version -Descending |
            Select-Object -First 1
        }
    }
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
