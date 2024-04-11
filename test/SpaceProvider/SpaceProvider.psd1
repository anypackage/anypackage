@{
    RootModule = 'SpaceProvider.psm1'
    ModuleVersion = '0.1.0'
    CompatiblePSEditions = @('Desktop', 'Core')
    GUID = '512d6b8c-7a6a-4ea3-bcca-a5ac70aa2493'
    Author = 'Thomas Nieto'
    Copyright = '(c) Thomas Nieto. All rights reserved.'
    Description = 'Test provider for AnyPackage.'
    PowerShellVersion = '5.1'
    RequiredModules = @('AnyPackage')
    FunctionsToExport = @()
    CmdletsToExport = @()
    AliasesToExport = @()
    PrivateData = @{
        AnyPackage = @{ Providers = 'Space Provider' }
        PSData = @{ }
    }
}
