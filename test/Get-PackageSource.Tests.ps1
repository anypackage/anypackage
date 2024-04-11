#requires -modules AnyPackage

[Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseDeclaredVarsMoreThanAssignments',
    '',
    Justification = 'Does not work with Pester scopes.')]
param()

Describe Get-PackageSource {
    BeforeAll {
        Import-Module (Join-Path -Path $PSScriptRoot -ChildPath PowerShellProvider)
    }

    Context 'with no additional parameters' {
        It 'should return sources' {
            Get-PackageSource |
            Should -Not -BeNullOrEmpty
        }
    }

    Context 'with -Name parameter' {
        It 'should return <_> source' -ForEach 'Default', 'D*' {
            Get-PackageSource -Name $_ |
            Should -Not -BeNullOrEmpty
        }

        It 'should return correct properties for <_> source' -ForEach 'Default' {
            $provider = Get-PackageProvider
            $source = Get-PackageSource

            $source.Name | Should -Be $provider.Sources.Name
            $source.Location | Should -Be $provider.Sources.Location
            $source.Trusted | Should -Be $provider.Sources.Trusted
        }
    }

    Context 'with pipeline input' {
        It 'should return <_> source' -ForEach 'Default' {
            $_ |
            Get-PackageSource |
            Should -Not -BeNullOrEmpty
        }
    }

    Context 'provider completer' {
        BeforeDiscovery {
            $completerTests = @(
                @{ Completion = 'D'; Source = 'Default'; Name = 'Default'; Count = 1 }    
                @{ Completion = 'Space'; Source = "'Space Source'"; Name = 'Space Source'; Count = 1 }
            )
        }

        BeforeAll {
            Register-PackageSource -Name 'Space Source' -Location TempDrive: -Provider PowerShell
        }

        AfterAll {
            Unregister-PackageSource -Name 'Space Source'
        }
        
        It 'should return all registered sources' {
            TabExpansion2 'Get-PackageSource -Name ' |
            Select-Object -ExpandProperty CompletionMatches |
            Measure-Object |
            Select-Object -ExpandProperty Count |
            Should -BeExactly 2
        }

        It 'should return correct completion properties for <Source>' -ForEach $completerTests {
            $results = TabExpansion2 "Get-PackageSource -Name $($_.Completion)" |
            Select-Object -ExpandProperty CompletionMatches

            $results | 
            Select-Object -ExpandProperty CompletionText |
            Should -BeExactly $_.Source

            $results |
            Select-Object -ExpandProperty ListItemText |
            Should -BeExactly $_.Name

            $results |
            Should -HaveCount $_.Count
        }
    }
}
