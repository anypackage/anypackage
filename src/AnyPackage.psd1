@{
    RootModule = 'AnyPackage.dll'
    ModuleVersion = '0.5.0'
    CompatiblePSEditions = @('Desktop', 'Core')
    GUID = '19cd4cdd-6766-4e47-be1c-76c33cea7392'
    Author = 'Thomas Nieto'
    Copyright = '(c) 2023 Thomas Nieto. All rights reserved.'
    Description = 'Use various package managers with a single set of commands.'
    PowerShellVersion = '5.1'
    FormatsToProcess = @('AnyPackage.format.ps1xml')
    FunctionsToExport = @()
    AliasesToExport = @()
    CmdletsToExport = @('Find-Package', 'Get-Package', 'Get-PackageProvider', 'Get-PackageSource',
                        'Install-Package', 'Publish-Package', 'Register-PackageSource',
                        'Save-Package', 'Set-PackageSource', 'Uninstall-Package',
                        'Unregister-PackageSource', 'Update-Package')
    PrivateData = @{
        PSData = @{
            Tags = @('Package', 'Manager', 'Windows', 'Linux', 'MacOS')
            LicenseUri = 'https://github.com/AnyPackage/AnyPackage/blob/main/LICENSE'
            ProjectUri = 'https://github.com/AnyPackage/AnyPackage'
        }
    }
    HelpInfoUri = 'https://go.anypackage.dev/help'
}
