#requires -modules AnyPackage

using module AnyPackage
using namespace AnyPackage.Provider

[Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseDeclaredVarsMoreThanAssignments',
    '',
    Justification = 'Does not work with Pester scopes.')]
param()

Describe PackageVersion {
    Context 'ToVersion() method' {
        BeforeDiscovery {
            $messages = @{
                AlphaNumeric = 'Exception calling "ToVersion" with "0" argument(s): "Cannot convert alpha-numeric versions to Version type."'
                TooManyParts = 'Exception calling "ToVersion" with "0" argument(s): "Version contains more than four parts."'
                TooFewParts  = 'Exception calling "ToVersion" with "0" argument(s): "Version contains less than two parts."'
                Prerelease   = 'Exception calling "ToVersion" with "0" argument(s): "Version contains prerelease."'
                Metadata     = 'Exception calling "ToVersion" with "0" argument(s): "Version contains build metadata."'
            }

            $badVersions = @(
                @{ Version = 'abc'; Message = $messages.AlphaNumeric }
                @{ Version = '1.2.3.4.5'; Message = $messages.TooManyParts }
                @{ Version = '1'; Message = $messages.TooFewParts }
                @{ Version = '1.2.3-alpha'; Message = $messages.Prerelease }
                @{ Version = '1.2.3+build'; Message = $messages.Metadata }
            )

            $goodVersions = @('1.0', '1.2', '1.2.3', '1.2.3.4')
        }

        It 'version <version> should error' -ForEach $badVersions {
            { [AnyPackage.Provider.PackageVersion]::new($Version).ToVersion() } |
                Should -Throw -ExpectedMessage $Message
        }

        It 'version <_> should not error' -ForEach $goodVersions {
            { [AnyPackage.Provider.PackageVersion]::new($_).ToVersion() } |
                Should -Not -Throw
        }
    }
}
