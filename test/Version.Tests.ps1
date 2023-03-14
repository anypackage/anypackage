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

    Context 'CompareTo() method' {
        BeforeDiscovery {
            $versions = @(
                @{ Version = '1.0'; Other = $null; Result = 1 }
                @{ Version = '1.0'; Other = '1.0'; Result = 0 }
                @{ Version = '1.0'; Other = '2.0'; Result = -1 }
                @{ Version = '2.0'; Other = '1.0'; Result = 1 }
                @{ Version = '1.0'; Other = '1.1'; Result = -1 }
                @{ Version = '1.1'; Other = '1.0'; Result = 1 }
                @{ Version = '1.0.0'; Other = '2.0.0'; Result = -1 }
                @{ Version = '2.0.0'; Other = '1.0.0'; Result = 1 }
                @{ Version = '1.0.1'; Other = '2.0'; Result = -1 }
                @{ Version = '2.0.0'; Other = '1.0.1'; Result = 1 }
                @{ Version = '1.2.3.4.5'; Other = '1.2.3.4.5'; Result = 0 }
                @{ Version = '1.2.3.4.5.1'; Other = '1.2.3.4.5'; Result = 1 }
                @{ Version = '1.2.3.4.5'; Other = '1.2.3.4.5.1'; Result = -1 }
                @{ Version = '1.0.0'; Other = '1.0.0-alpha'; Result = 1 }
                @{ Version = '1.0.0-alpha'; Other = '1.0.0'; Result = -1 }
                @{ Version = '1.0.0-alpha'; Other = '1.0.0-alpha'; Result = 0 }
                @{ Version = '1'; Other = '1.0.0-alpha'; Result = 1 }
                @{ Version = '1.0.0-alpha'; Other = '1'; Result = -1 }
                @{ Version = '1.0.0-alpha.1'; Other = '1.0.0-alpha.1'; Result = 0 }
                @{ Version = '1.0.0-alpha.1'; Other = '1.0.0-alpha.2'; Result = -1 }
                @{ Version = '1.0.0-alpha.2'; Other = '1.0.0-alpha.1'; Result = 1 }
                @{ Version = '1.0.0-alpha.1.1'; Other = '1.0.0-alpha.1'; Result = 1 }
                @{ Version = '1.0.0-alpha.1'; Other = '1.0.0-alpha.1.1'; Result = -1 }
                @{ Version = '1.0.0-alpha.1'; Other = '1.0.0-alpha.2'; Result = -1 }
                @{ Version = '1.0a'; Other = '1.0a'; Result = 0 }
                @{ Version = '1.0a'; Other = '1.0b'; Result = -1 }
                @{ Version = '1.0b'; Other = '1.0a'; Result = 1 }
                @{ Version = '1.0'; Other = '1.0a'; Result = -1 }
                @{ Version = '1.0a'; Other = '1.0'; Result = 1 }
                @{ Version = '1.0.0a'; Other = '1.0.0-alpha'; Result = 1 }
                @{ Version = '1.0.0-alpha'; Other = '1.0.0a'; Result = -1 }
                @{ Version = '1.0'; Other = 'abc'; Result = -1 }
                @{ Version = 'abc'; Other = '1.0'; Result = 1 }
                @{ Version = 'abc'; Other = 'abc'; Result = 0 }
                @{ Version = 'abc'; Other = 'def'; Result = -1 }
                @{ Version = 'def'; Other = 'abc'; Result = 1 }
            )
        }

        It "should return '<Result>' for '<Version>' compare to '<Other>'" -ForEach $versions {
            $a = [AnyPackage.Provider.PackageVersion]$Version
            $b = [AnyPackage.Provider.PackageVersion]$Other
            
            $a.CompareTo($b) | Should -Be $Result
        }
    }
}
