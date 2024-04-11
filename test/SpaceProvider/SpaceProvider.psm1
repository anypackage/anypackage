using module AnyPackage

using namespace AnyPackage.Provider
using namespace System.Collections.Generic

[PackageProvider('Space Provider')]
class SpaceProvider : PackageProvider {

}

[guid] $id = '91b3d5c7-d30c-4b8f-a9f4-5900fa96ac91'

[PackageProviderManager]::RegisterProvider($id, [SpaceProvider], $MyInvocation.MyCommand.ScriptBlock.Module)

$MyInvocation.MyCommand.ScriptBlock.Module.OnRemove = {
    [PackageProviderManager]::UnregisterProvider($id)
}
