using module AnyPackage

using namespace AnyPackage.Provider
using namespace System.Collections.Generic

# TestProviderA is used to simulate multiple
# modules shipping the same provider name
[PackageProvider('Soda')]
class TestProviderB : PackageProvider {

}

[guid] $id = '70132d39-e533-4896-9e6a-b3f4c6d7fca4'

[PackageProviderManager]::RegisterProvider($id, [TestProviderB], $MyInvocation.MyCommand.ScriptBlock.Module)

$MyInvocation.MyCommand.ScriptBlock.Module.OnRemove = {
    [PackageProviderManager]::UnregisterProvider($id)
}
