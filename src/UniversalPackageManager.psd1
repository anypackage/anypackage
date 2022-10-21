@{
    RootModule = 'UniversalPackageManager.dll'
    ModuleVersion = '0.1.0'
    CompatiblePSEditions = @('Desktop', 'Core')
    GUID = '19cd4cdd-6766-4e47-be1c-76c33cea7392'
    Author = 'Thomas Nieto'
    Copyright = '(c) 2019 Thomas Nieto. All rights reserved.'
    Description = 'Use various package managers with a single set of commands.'
    PowerShellVersion = '5.1'
    RequiredAssemblies = @('NuGet.Versioning.dll', 'System.Collections.Immutable.dll')
    FormatsToProcess = @('UniversalPackageManager.format.ps1xml')
    FunctionsToExport = @()
    AliasesToExport = @()
    CmdletsToExport = @('Find-Package', 'Get-Package', 'Get-PackageProvider', 'Get-PackageSource',
                        'Install-Package', 'Publish-Package', 'Register-PackageSource',
                        'Save-Package', 'Set-PackageSource', 'Uninstall-Package',
                        'Unregister-PackageSource', 'Update-Package')
    PrivateData = @{
        PSData = @{
            Tags = @('Package', 'Manager', 'Windows', 'Linux', 'MacOS')
            LicenseUri = 'https://github.com/ThomasNieto/UniversalPackageManager/blob/master/LICENSE'
            ProjectUri = 'https://github.com/ThomasNieto/UniversalPackageManager'
        }
    }
}
