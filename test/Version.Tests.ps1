#requires -modules AnyPackage

using module AnyPackage
using namespace AnyPackage.Provider

[Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseDeclaredVarsMoreThanAssignments',
    '',
    Justification = 'Does not work with Pester scopes.')]
param()

Describe PackageVersion {
    Context 'Constructor' {
        BeforeDiscovery {
            $versions = @(
                @{
                    Version      = '1.2.3'
                    Scheme       = 'SemanticVersion'
                    Major        = 1
                    Minor        = 2
                    Patch        = 3
                    Revision     = $null
                    Parts        = @(1, 2, 3)
                    Suffix       = $null
                    IsPrerelease = $false
                    Prerelease   = @()
                    HasMetadata  = $false
                    Metadata     = @()
                }
                @{
                    Version      = '1.2.3-alpha'
                    Scheme       = 'SemanticVersion'
                    Major        = 1
                    Minor        = 2
                    Patch        = 3
                    Revision     = $null
                    Parts        = @(1, 2, 3)
                    Suffix       = $null
                    IsPrerelease = $true
                    Prerelease   = @('alpha')
                    HasMetadata  = $false
                    Metadata     = @()
                }
                @{
                    Version      = '1.2.3-alpha.1'
                    Scheme       = 'SemanticVersion'
                    Major        = 1
                    Minor        = 2
                    Patch        = 3
                    Revision     = $null
                    Parts        = @(1, 2, 3)
                    Suffix       = $null
                    IsPrerelease = $true
                    Prerelease   = @('alpha', 1)
                    HasMetadata  = $false
                    Metadata     = @()
                }
                @{
                    Version      = '1.2.3+build'
                    Scheme       = 'SemanticVersion'
                    Major        = 1
                    Minor        = 2
                    Patch        = 3
                    Revision     = $null
                    Parts        = @(1, 2, 3)
                    Suffix       = $null
                    IsPrerelease = $false
                    Prerelease   = @()
                    HasMetadata  = $true
                    Metadata     = @('build')
                }
                @{
                    Version      = '1.2.3+build.1'
                    Scheme       = 'SemanticVersion'
                    Major        = 1
                    Minor        = 2
                    Patch        = 3
                    Revision     = $null
                    Parts        = @(1, 2, 3)
                    Suffix       = $null
                    IsPrerelease = $false
                    Prerelease   = @()
                    HasMetadata  = $true
                    Metadata     = @('build', 1)
                }
                @{
                    Version      = '1.2.3-alpha.1+build.1'
                    Scheme       = 'SemanticVersion'
                    Major        = 1
                    Minor        = 2
                    Patch        = 3
                    Revision     = $null
                    Parts        = @(1, 2, 3)
                    Suffix       = $null
                    IsPrerelease = $true
                    Prerelease   = @('alpha', 1)
                    HasMetadata  = $true
                    Metadata     = @('build', 1)
                }
                @{
                    Version      = '1'
                    Scheme       = 'Integer'
                    Major        = 1
                    Minor        = $null
                    Patch        = $null
                    Revision     = $null
                    Parts        = @(1)
                    Suffix       = $null
                    IsPrerelease = $false
                    Prerelease   = @()
                    HasMetadata  = $false
                    Metadata     = @()
                }
                @{
                    Version      = '1.2.3a'
                    Scheme       = 'MultiPartNumericSuffix'
                    Major        = 1
                    Minor        = 2
                    Patch        = 3
                    Revision     = $null
                    Parts        = @(1, 2, 3)
                    Suffix       = 'a'
                    IsPrerelease = $false
                    Prerelease   = @()
                    HasMetadata  = $false
                    Metadata     = @()
                }
                @{
                    Version      = '1.2'
                    Scheme       = 'MultiPartNumeric'
                    Major        = 1
                    Minor        = 2
                    Patch        = $null
                    Revision     = $null
                    Parts        = @(1, 2)
                    Suffix       = $null
                    IsPrerelease = $false
                    Prerelease   = @()
                    HasMetadata  = $false
                    Metadata     = @()
                }
                @{
                    Version      = '1.2.3.4'
                    Scheme       = 'MultiPartNumeric'
                    Major        = 1
                    Minor        = 2
                    Patch        = 3
                    Revision     = 4
                    Parts        = @(1, 2, 3, 4)
                    Suffix       = $null
                    IsPrerelease = $false
                    Prerelease   = @()
                    HasMetadata  = $false
                    Metadata     = @()
                }
                @{
                    Version      = '1.2.3.4.5.6'
                    Scheme       = 'MultiPartNumeric'
                    Major        = 1
                    Minor        = 2
                    Patch        = 3
                    Revision     = 4
                    Parts        = @(1, 2, 3, 4, 5, 6)
                    Suffix       = $null
                    IsPrerelease = $false
                    Prerelease   = @()
                    HasMetadata  = $false
                    Metadata     = @()
                }
                @{
                    Version      = 'abc'
                    Scheme       = 'AlphaNumeric'
                    Major        = $null
                    Minor        = $null
                    Patch        = $null
                    Revision     = $null
                    Parts        = @()
                    Suffix       = $null
                    IsPrerelease = $false
                    Prerelease   = @()
                    HasMetadata  = $false
                    Metadata     = @()
                }
                @{
                    Version      = [version]'5.6'
                    Scheme       = 'MultiPartNumeric'
                    Major        = 5
                    Minor        = 6
                    Patch        = $null
                    Revision     = $null
                    Parts        = @(5, 6)
                    Suffix       = $null
                    IsPrerelease = $false
                    Prerelease   = @()
                    HasMetadata  = $false
                    Metadata     = @()
                }
                @{
                    Version      = [version]'5.6.7'
                    Scheme       = 'MultiPartNumeric'
                    Major        = 5
                    Minor        = 6
                    Patch        = 7
                    Revision     = $null
                    Parts        = @(5, 6, 7)
                    Suffix       = $null
                    IsPrerelease = $false
                    Prerelease   = @()
                    HasMetadata  = $false
                    Metadata     = @()
                }
                @{
                    Version      = [version]'5.6.7.8'
                    Scheme       = 'MultiPartNumeric'
                    Major        = 5
                    Minor        = 6
                    Patch        = 7
                    Revision     = 8
                    Parts        = @(5, 6, 7, 8)
                    Suffix       = $null
                    IsPrerelease = $false
                    Prerelease   = @()
                    HasMetadata  = $false
                    Metadata     = @()
                }
            )
        }

        It 'should return correct properties for <Version> version' -ForEach $versions {
            $v = [AnyPackage.Provider.PackageVersion]::new($Version)
            $v.Scheme | Should -Be $Scheme
            $v.Major | Should -Be $Major
            $v.Minor | Should -Be $Minor
            $v.Patch | Should -Be $Patch
            $v.Parts | Should -Be $Parts
            $v.Suffix | Should -Be $Suffix
            $v.IsPrerelease | Should -Be $IsPrerelease
            $v.HasMetadata | Should -Be $HasMetadata
            $v.Prerelease | Should -Be $Prerelease
            $v.Metadata | Should -Be $Metadata
        }
    }
    
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

        It 'should error for <version> version' -ForEach $badVersions {
            { [AnyPackage.Provider.PackageVersion]::new($Version).ToVersion() } |
                Should -Throw -ExpectedMessage $Message
        }

        It 'should work for <_> version' -ForEach $goodVersions {
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
