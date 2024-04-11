@{
    RootModule = 'TestProviderB.psm1'
    ModuleVersion = '0.1.0'
    CompatiblePSEditions = @('Desktop', 'Core')
    GUID = '713922da-12b2-4637-939a-04f91efbe385'
    Author = 'Thomas Nieto'
    Copyright = '(c) Thomas Nieto. All rights reserved.'
    Description = 'Test provider for AnyPackage.'
    PowerShellVersion = '5.1'
    RequiredModules = @('AnyPackage')
    FunctionsToExport = @()
    CmdletsToExport = @()
    AliasesToExport = @()
    PrivateData = @{
        AnyPackage = @{ Providers = 'Soda' }
        PSData = @{ }
    }
}
