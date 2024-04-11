using module AnyPackage

using namespace AnyPackage.Provider
using namespace System.Collections.Generic

[PackageProvider('Soda')]
class TestProviderA : PackageProvider {

}

[guid] $id = 'a1756b3f-310e-4a67-a17a-ae80bad99cbe'

[PackageProviderManager]::RegisterProvider($id, [TestProviderA], $MyInvocation.MyCommand.ScriptBlock.Module)

$MyInvocation.MyCommand.ScriptBlock.Module.OnRemove = {
    [PackageProviderManager]::UnregisterProvider($id)
}
