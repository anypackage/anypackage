using module AnyPackage

using namespace AnyPackage.Feedback
using namespace AnyPackage.Provider
using namespace System.Collections.Generic
using namespace System.Threading

[PackageProvider('PowerShell', FileExtensions = ('.json'), UriSchemes = ('file'))]
class PowerShellProvider : PackageProvider,
    IFindPackage, IGetPackage,
    IInstallPackage, IPublishPackage,
    ISavePackage, IUninstallPackage, IUpdatePackage, IOptimizePackage,
    IGetSource, ISetSource, ICommandNotFound {
    [PackageProviderInfo] Initialize([PackageProviderInfo] $providerInfo) {
        return [PowerShellProviderInfo]::new($providerInfo)
    }

    [bool] IsSource([string] $source) {
        if ($source -eq 'broke') {
            return $false
        }
        else {
            return $true
        }
    }

    [IEnumerable[CommandNotFoundFeedback]] FindPackage([CommandNotFoundContext] $context, [CancellationToken] $token) {
        $dict = New-Object "System.Collections.Generic.Dictionary[[string],[CommandNotFoundFeedback]]"
        foreach ($source in $this.ProviderInfo.Sources) {
            $path = Join-Path -Path $source.Location -ChildPath *.json
            Get-ChildItem -Path $path |
            ForEach-Object {
                Get-Content -Path $_.FullName |
                ConvertFrom-Json |
                Where-Object { $_.bin -contains $context.Command } |
                ForEach-Object {
                    if (!$dict.ContainsKey($_.name)) {
                        $feedback = [CommandNotFoundFeedback]::new($_.name, $this.ProviderInfo)
                        $dict.Add($_.name, $feedback)
                    }
                }
            }
        }

        return $dict.Values
    }

    [void] FindPackage([PackageRequest] $request) {
        if ($request.Path) {
            $this.FindPackageByFile($request)
            return
        }
        elseif ($request.Uri) {
            $this.FindPackageByUri($request)
            return
        }

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
                Write-Package -Request $request -Source $source -Provider $this.ProviderInfo
            }
        }
    }

    [void] FindPackageByFile([PackageRequest] $request) {
        Get-Content -LiteralPath $request.Path |
        ConvertFrom-Json |
        Write-Package -Request $request -Provider $this.ProviderInfo
    }

    [void] FindPackageByUri([PackageRequest] $request) {
        Get-Content -LiteralPath $request.Uri.LocalPath |
        ConvertFrom-Json |
        Write-Package -Request $request -Provider $this.ProviderInfo
    }

    [void] GetPackage([PackageRequest] $request) {
        $this.ProviderInfo.Packages |
        Where-Object { $_ } |
        Where-Object { $request.IsMatch($_.Name, $_.version) } |
        ForEach-Object {
            $_ | Write-Package -Request $request -Source $_.Source -Provider $this.ProviderInfo
        }
    }

    [void] InstallPackage([PackageRequest] $request) {
        if ($request.Path) {
            $this.InstallPackageByFile($request)
            return
        }
        elseif ($request.Uri) {
            $this.InstallPackageByUri($request)
            return
        }

        $params = @{
            Name = $request.Name
            Prerelease = $request.Prerelease
            Provider = 'PowerShell'
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
            $_ | Write-Package -Request $request -Source $_.Source -Provider $this.ProviderInfo
        }
    }

    [void] InstallPackageByFile([PackageRequest] $request) {
        Find-Package -LiteralPath $request.Path |
        ForEach-Object {
            $this.ProviderInfo.Packages += $_
            $_ | Write-Package -Request $request -Provider $this.ProviderInfo
        }
    }

    [void] InstallPackageByUri([PackageRequest] $request) {
        Find-Package -Uri $request.Uri |
        ForEach-Object {
            $this.ProviderInfo.Packages += $_
            $_ | Write-Package -Request $request -Provider $this.ProviderInfo
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
        Write-Package -Request $request -Source $source -Provider $this.ProviderInfo
    }

    [void] OptimizePackage([PackageRequest] $request) {
        $packages = Get-Package -Name $request.Name -Provider $this.ProviderInfo -ErrorAction SilentlyContinue |
        Group-Object -Property Name

        foreach ($package in $packages) {
            if ($package.Count -gt 1) {
                $package.Group |
                Sort-Object -Property Version -Descending |
                Select-Object -Skip 1 |
                Uninstall-Package -PassThru |
                ForEach-Object { $request.WritePackage($_) }
            }
        }
    }

    [void] SavePackage([PackageRequest] $request) {
        $params = @{
            Name = $request.Name
            Prerelease = $request.Prerelease
            Provider = 'PowerShell'
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
            $path = Join-Path -Path $_.Source.Location -ChildPath ("$($_.Name)-$($_.Version).json").ToLower()
            Copy-Item -Path $path -Destination $request.Path

            $_ | Write-Package -Request $request -Source $_.Source -Provider $this.ProviderInfo
        }
    }

    [void] UninstallPackage([PackageRequest] $request) {
        $this.ProviderInfo.Packages = $this.ProviderInfo.Packages |
        Where-Object { $_ } |
        ForEach-Object {
            if ($request.IsMatch($_.Name, $_.Version)) {
                $_ |
                Write-Package -Request $request -Source $_.Source -Provider $this.ProviderInfo
            }
            else {
                $_
            }
        }
    }

    [void] UpdatePackage([PackageRequest] $request) {
        if ($request.Path) {
            $this.UpdatePackageByFile($request)
            return
        }
        elseif ($request.Uri)
        {
            $this.UpdatePackageByUri($request)
            return
        }

        $getPackageParams = @{
            Name = $request.Name
            Provider = 'PowerShell'
            ErrorAction = 'Ignore'
        }

        $findPackageParams = @{
            Prerelease = $request.Prerelease
            ErrorAction = 'Ignore'
        }

        if ($request.Version) {
            $findPackageParams['Version'] = $request.Version
        }

        if ($request.Source) {
            $findPackageParams['Source'] = $request.Source
        }

        Get-Package @getPackageParams |
        Select-Object -Property Name -Unique |
        Find-Package @findPackageParams |
        Get-Latest |
        ForEach-Object {
            $this.ProviderInfo.Packages += $_

            $_ | Write-Package -Request $request -Source $_.Source -Provider $this.ProviderInfo
        }
    }

    [void] UpdatePackageByFile([PackageRequest] $request) {
        Find-Package -LiteralPath $request.Path |
        ForEach-Object {
            $this.ProviderInfo.Packages += $_
            $_ | Write-Package -Request $request -Provider $this.ProviderInfo
        }
    }

    [void] UpdatePackageByUri([PackageRequest] $request) {
        Find-Package -Uri $request.Uri |
        ForEach-Object {
            $this.ProviderInfo.Packages += $_
            $_ | Write-Package -Request $request -Provider $this.ProviderInfo
        }
    }

    [void] GetSource([SourceRequest] $sourceRequest) {
        $this.ProviderInfo.Sources |
        Where-Object Name -like $sourceRequest.Name |
        Write-Source -SourceRequest $sourceRequest -Provider $this.ProviderInfo
    }

    [void] RegisterSource([SourceRequest] $sourceRequest) {
        $source = [PSCustomObject]@{
            Name = $sourceRequest.Name
            Location = $sourceRequest.Location
            Trusted = $sourceRequest.Trusted
        }

        $this.ProviderInfo.Sources += $source

        $source |
        Write-Source -SourceRequest $sourceRequest -Provider $this.ProviderInfo
    }

    [void] SetSource([SourceRequest] $sourceRequest) {
        $source = $this.ProviderInfo.Sources |
        Where-Object { $_.Name -eq $sourceRequest.Name }

        if (-not $source) { return }

        if ($sourceRequest.Location) {
            $source.Location = $sourceRequest.Location
        }

        if ($null -ne $sourceRequest.Trusted) {
            $source.Trusted = $sourceRequest.Trusted
        }

        $source |
        Write-Source -SourceRequest $sourceRequest -Provider $this.ProviderInfo
    }

    [void] UnregisterSource([SourceRequest] $sourceRequest) {
        $this.ProviderInfo.Sources = $this.ProviderInfo.Sources |
        ForEach-Object {
            if ($sourceRequest.Name -eq $_.Name) {
                $_ |
                Write-Source -SourceRequest $sourceRequest -Provider $this.ProviderInfo
            }
            else {
                $_
            }
        }
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

[guid] $id = '89d76409-f1b0-46cb-a881-b012be54aef5'

[PackageProviderManager]::RegisterProvider($id, [PowerShellProvider], $MyInvocation.MyCommand.ScriptBlock.Module)

$MyInvocation.MyCommand.ScriptBlock.Module.OnRemove = {
    [PackageProviderManager]::UnregisterProvider($id)
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
        $SourceRequest,

        [Parameter(Mandatory)]
        [PackageProviderInfo]
        $Provider
    )

    process {
        $source = [PackageSourceInfo]::new($InputObject.Name, $InputObject.Location, $InputObject.Trusted, $Provider)
        $SourceRequest.WriteSource($source)
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
        $Source,

        [Parameter(Mandatory)]
        [PackageProviderInfo]
        $Provider
    )

    process {
        if ($Source) {
            $Source = [PackageSourceInfo]::new($Source.Name, $Source.Location, $Source.Trusted, $Provider)
        }

        $package = [PackageInfo]::new($InputObject.Name,
                                      $InputObject.Version,
                                      $Source,
                                      $InputObject.Description,
                                      $Provider)

        $Request.WritePackage($package)
    }
}
