#requires -modules AnyPackage

using module AnyPackage
using namespace AnyPackage.Provider

[Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseDeclaredVarsMoreThanAssignments',
    '',
    Justification = 'Does not work with Pester scopes.')]
param()

Describe VersionRange {
    Context 'Constructor(range)' {
        BeforeDiscovery {
            $ranges = @(
                @{
                    Range          = '1.0'
                    MinVersion     = '1.0'
                    MaxVersion     = '1.0'
                    IsMinInclusive = $true
                    IsMaxInclusive = $true
                    String         = '[1.0]'
                }
                @{
                    Range          = '[1.0]'
                    MinVersion     = '1.0'
                    MaxVersion     = '1.0'
                    IsMinInclusive = $true
                    IsMaxInclusive = $true
                    String         = '[1.0]'
                }
                @{
                    Range          = '[1.0,2.0]'
                    MinVersion     = '1.0'
                    MaxVersion     = '2.0'
                    IsMinInclusive = $true
                    IsMaxInclusive = $true
                    String         = '[1.0,2.0]'
                }
                @{
                    Range          = '(1.0,2.0)'
                    MinVersion     = '1.0'
                    MaxVersion     = '2.0'
                    IsMinInclusive = $false
                    IsMaxInclusive = $false
                    String         = '(1.0,2.0)'
                }
                @{
                    Range          = '[1.0,)'
                    MinVersion     = '1.0'
                    MaxVersion     = $null
                    IsMinInclusive = $true
                    IsMaxInclusive = $false
                    String         = '[1.0,)'
                }
                @{
                    Range          = '(1.0,)'
                    MinVersion     = '1.0'
                    MaxVersion     = $null
                    IsMinInclusive = $false
                    IsMaxInclusive = $false
                    String         = '(1.0,)'
                }
                @{
                    Range          = '(,2.0]'
                    MinVersion     = $null
                    MaxVersion     = '2.0'
                    IsMinInclusive = $false
                    IsMaxInclusive = $true
                    String         = '(,2.0]'
                }
                @{
                    Range          = '(,2.0)'
                    MinVersion     = $null
                    MaxVersion     = '2.0'
                    IsMinInclusive = $false
                    IsMaxInclusive = $false
                    String         = '(,2.0)'
                }
            )

            $rangeVersions = @(
                @{
                    MinVersion     = [AnyPackage.Provider.PackageVersion]'1.0'
                    MaxVersion     = [AnyPackage.Provider.PackageVersion]'1.0'
                    IsMinInclusive = $true
                    IsMaxInclusive = $false
                    String         = '[1.0]'
                }
            )

            $badRanges = @('[,]', '(,)', '[2.0,1.0]')
        }

        It 'should return correct properties for <Range> range' -ForEach $ranges {
            $r = [AnyPackage.Provider.PackageVersionRange]::new($Range)
            $r.ToString() | Should -Be $String
            $r.IsMinInclusive | Should -Be $IsMinInclusive
            $r.IsMaxInclusive | Should -Be $IsMaxInclusive

            if ($MinVersion) {
                $r.MinVersion | Should -Be $MinVersion
            }

            if ($MaxVersion) {
                $r.MaxVersion | Should -Be $MaxVersion
            }
        }

        It 'should error for <_> range' -ForEach $badRanges {
            { [AnyPackage.Provider.PackageVersionRange]::new($_) } |
                Should -Throw
        }
    }

    Context 'Constructor(minVersion, maxVersion)' {
        BeforeDiscovery {
            $rangeVersions = @(
                @{
                    MinVersion     = [AnyPackage.Provider.PackageVersion]'1.0'
                    MaxVersion     = [AnyPackage.Provider.PackageVersion]'1.0'
                    IsMinInclusive = $true
                    IsMaxInclusive = $true
                    String         = '[1.0]'
                }
                @{
                    MinVersion     = [AnyPackage.Provider.PackageVersion]'1.0'
                    MaxVersion     = [AnyPackage.Provider.PackageVersion]'2.0'
                    IsMinInclusive = $true
                    IsMaxInclusive = $true
                    String         = '[1.0,2.0]'
                }
                @{
                    MinVersion     = [AnyPackage.Provider.PackageVersion]'1.0'
                    MaxVersion     = [AnyPackage.Provider.PackageVersion]'2.0'
                    IsMinInclusive = $true
                    IsMaxInclusive = $false
                    String         = '[1.0,2.0)'
                }
                @{
                    MinVersion     = [AnyPackage.Provider.PackageVersion]'1.0'
                    MaxVersion     = [AnyPackage.Provider.PackageVersion]'2.0'
                    IsMinInclusive = $false
                    IsMaxInclusive = $true
                    String         = '(1.0,2.0]'
                }
                @{
                    MinVersion     = [AnyPackage.Provider.PackageVersion]'1.0'
                    MaxVersion     = $null
                    IsMinInclusive = $true
                    IsMaxInclusive = $false
                    String         = '[1.0,)'
                }
                @{
                    MinVersion     = [AnyPackage.Provider.PackageVersion]'1.0'
                    MaxVersion     = $null
                    IsMinInclusive = $false
                    IsMaxInclusive = $false
                    String         = '(1.0,)'
                }
                @{
                    MinVersion     = $null
                    MaxVersion     = [AnyPackage.Provider.PackageVersion]'2.0'
                    IsMinInclusive = $false
                    IsMaxInclusive = $true
                    String         = '(,2.0]'
                }
                @{
                    MinVersion     = $null
                    MaxVersion     = [AnyPackage.Provider.PackageVersion]'2.0'
                    IsMinInclusive = $false
                    IsMaxInclusive = $false
                    String         = '(,2.0)'
                }
            )

            $badRangeVersions = @(
                @{
                    MinVersion     = [AnyPackage.Provider.PackageVersion]'2.0'
                    MaxVersion     = [AnyPackage.Provider.PackageVersion]'1.0'
                    IsMinInclusive = $true
                    IsMaxInclusive = $true
                    String         = '[2.0,1.0]'
                }
            )
        }

        It 'should return correct properties for <String> range' -ForEach $rangeVersions {
            $r = [AnyPackage.Provider.PackageVersionRange]::new($MinVersion, $MaxVersion, $IsMinInclusive, $IsMaxInclusive)
            $r.ToString() | Should -Be $String
            $r.IsMinInclusive | Should -Be $IsMinInclusive
            $r.IsMaxInclusive | Should -Be $IsMaxInclusive

            if ($MinVersion) {
                $r.MinVersion | Should -Be $MinVersion
            }

            if ($MaxVersion) {
                $r.MaxVersion | Should -Be $MaxVersion
            }
        }

        It 'should error for <String> range' -ForEach $badRangeVersions {
            { [AnyPackage.Provider.PackageVersionRange]::new($MinVersion, $MaxVersion, $IsMinInclusive, $IsMaxInclusive) } |
                Should -Throw
        }
    }
    
    Context 'Satisfies(version) method' {
        BeforeDiscovery {
            $tests = @(
                @{
                    Range   = '1.0'
                    Version = '1.0'
                    Result  = $true
                }
                @{
                    Range   = '1.0'
                    Version = '0.1.0'
                    Result  = $false
                }
                @{
                    Range   = '1.0'
                    Version = '2.0'
                    Result  = $false
                }
                @{
                    Range   = '[1.0]'
                    Version = '1.0'
                    Result  = $true
                }
                @{
                    Range   = '[1.0]'
                    Version = '0.1.0'
                    Result  = $false
                }
                @{
                    Range   = '[1.0]'
                    Version = '2.0'
                    Result  = $false
                }
                @{
                    Range   = '(1.0,)'
                    Version = '1.0'
                    Result  = $false
                }
                @{
                    Range   = '(1.0,)'
                    Version = '2.0'
                    Result  = $true
                }
                @{
                    Range   = '[1.0,)'
                    Version = '0.1.0'
                    Result  = $false
                }
                @{
                    Range   = '[1.0,)'
                    Version = '1.0'
                    Result  = $true
                }
                @{
                    Range   = '[1.0,)'
                    Version = '2.0'
                    Result  = $true
                }
                @{
                    Range   = '(,2.0)'
                    Version = '2.0'
                    Result  = $false
                }
                @{
                    Range   = '(,2.0)'
                    Version = '1.0'
                    Result  = $true
                }
                @{
                    Range   = '(,2.0]'
                    Version = '2.0'
                    Result  = $true
                }
                @{
                    Range   = '(,2.0]'
                    Version = '1.0'
                    Result  = $true
                }
                @{
                    Range   = '(,2.0]'
                    Version = '2.1'
                    Result  = $false
                }
                @{
                    Range   = '(1.0,2.0)'
                    Version = '1.0'
                    Result  = $false
                }
                @{
                    Range   = '(1.0,2.0)'
                    Version = '2.0'
                    Result  = $false
                }
                @{
                    Range   = '(1.0,2.0)'
                    Version = '1.5'
                    Result  = $true
                }
                @{
                    Range   = '[1.0,2.0]'
                    Version = '1.0'
                    Result  = $true
                }
                @{
                    Range   = '(1.0,2.0)'
                    Version = '2.0'
                    Result  = $false
                }
                @{
                    Range   = '(1.0,2.0)'
                    Version = '0.1.0'
                    Result  = $false
                }
                @{
                    Range   = '(1.0,2.0)'
                    Version = '3.0'
                    Result  = $false
                }
                @{
                    Range   = '[1.0,2.0)'
                    Version = '1.0'
                    Result  = $true
                }
                @{
                    Range   = '[1.0,2.0)'
                    Version = '0.1.0'
                    Result  = $false
                }
                @{
                    Range   = '[1.0,2.0)'
                    Version = '3.0'
                    Result  = $false
                }
                @{
                    Range   = '(1.0,2.0]'
                    Version = '1.0'
                    Result  = $false
                }
                @{
                    Range   = '(1.0,2.0]'
                    Version = '2.0'
                    Result  = $true
                }
                @{
                    Range   = '(1.0,2.0]'
                    Version = '3.0'
                    Result  = $false
                }
            )
        }

        It 'should return <Result> for <Range> range and <Version> version' -ForEach $tests {
            [AnyPackage.Provider.PackageVersionRange]::new($Range).Satisfies($Version) |
            Should -Be $Result
        }
    }
}
