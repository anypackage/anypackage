@{
    RootModule = 'TestProviderA.psm1'
    ModuleVersion = '0.1.0'
    CompatiblePSEditions = @('Desktop', 'Core')
    GUID = 'fec74669-e7dd-4643-be29-a7e17ce1b373'
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
