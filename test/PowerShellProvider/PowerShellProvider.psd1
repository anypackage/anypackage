@{
    RootModule = 'PowerShellProvider.psm1'
    ModuleVersion = '0.1.0'
    CompatiblePSEditions = @('Desktop', 'Core')
    GUID = 'ae9da3f9-9780-4713-ac27-13ab4bfda078'
    Author = 'Thomas Nieto'
    Copyright = '(c) Thomas Nieto. All rights reserved.'
    Description = 'Test provider for AnyPackage.'
    PowerShellVersion = '5.1'
    RequiredModules = @('AnyPackage')
    FunctionsToExport = @()
    CmdletsToExport = @()
    AliasesToExport = @()
    PrivateData = @{
        AnyPackage = @{ Providers = 'PowerShell' }
        PSData = @{ }
    }
}
