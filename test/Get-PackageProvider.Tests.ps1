#requires -modules AnyPackage

[Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseDeclaredVarsMoreThanAssignments',
    '',
    Justification = 'Does not work with Pester scopes.')]
param()

Describe Get-PackageProvider {
    BeforeAll {
        Import-Module (Join-Path -Path $PSScriptRoot -ChildPath PowerShellProvider)
        Import-Module (Join-Path -Path $PSScriptRoot -ChildPath SpaceProvider)
        Import-Module (Join-Path -Path $PSScriptRoot -ChildPath TestProviderA)
        Import-Module (Join-Path -Path $PSScriptRoot -ChildPath TestProviderB)
    }

    AfterAll {
        Remove-Module PowerShellProvider, SpaceProvider, TestProviderA, TestProviderB
    }

    Context 'with no additional parameters' {
        It 'should return results' {
            Get-PackageProvider |
            Should -Not -BeNullOrEmpty
        }
    }

    Context 'with -Name parameter' {
        It 'should return <_> provider' -ForEach 'PowerShell', 'P*' {
            Get-PackageProvider -Name $_ |
            Should -Not -BeNullOrEmpty
        }

        It 'should error with non-existent package' -ForEach 'broke' {
            { Get-PackageProvider -Name $_ -ErrorAction Stop } |
            Should -Throw -ExpectedMessage "Package provider not found. (Provider '$_')"
        }

        It 'should not error with wildcard non-existent package' -ForEach 'broke*' {
            { Get-PackageProvider -Name $_ -ErrorAction Stop } |
            Should -Not -Throw
        }

        It 'should have correct properties for <Provider> provider' -ForEach @{ Provider = 'PowerShell'; Type = 'PowerShellProvider'; Module = 'PowerShellProvider' } {
            $result = Get-PackageProvider -Name $Provider

            $result.Name | Should -Be $Provider
            $result.Id | Should -Not -Be [guid]::Empty
            $result.Module.Name | Should -Be $Module
            $result.ModuleName | Should -Be $Module
            $result.FullName | Should -Be "$Module\$Provider"
            $result.ImplementingType.Name | Should -Be $Type
            $result.PackageByName | Should -BeTrue
            $result.PackageByFile | Should -BeTrue
            $result.PackageByUri | Should -BeTrue
            $result.FileExtensions | Should -Be @('.json')
            $result.UriSchemes | Should -Be @('file')
        }
    }

    Context 'with -ListAvailable parameter' {
        BeforeAll {
            $beforeModulePath = $env:PSModulePath
            
            if ($IsWindows) {
                $env:PSModulePath += ";$PSScriptRoot"
            }
            else {
                $env:PSModulePath += ":$PSScriptRoot"
            }
        }

        AfterAll {
            $env:PSModulePath = $beforeModulePath
        }
        
        It 'should return results' {
            Get-PackageProvider -ListAvailable |
            Should -Not -BeNullOrEmpty
        }

        It 'should return <_> provider' -ForEach 'PowerShell', 'P*', '*' {
            $results = Get-PackageProvider -Name $_ -ListAvailable

            $results | Should -Not -BeNullOrEmpty
            $results |
            Select-Object -ExpandProperty Name -Unique |
            Should -BeLike $_
        }
    }

    Context 'with derived class' {
        It 'should return derived class type' -ForEach @{ Provider = 'PowerShell'; Type = 'PowerShellProviderInfo' } {
            $result = Get-PackageProvider -Name $Provider
            $result.GetType().Name | Should -Be $Type
        }

        It 'should have <Property> property' -ForEach @{ Provider = 'PowerShell'; Property = @('Packages', 'Sources') } {
            $result = Get-PackageProvider -Name $Provider

            foreach ($prop in @($Property)) {
                $result |
                Get-Member -Name $prop |
                Select-Object -ExpandProperty Name |
                Should -Be $prop
            }
        }
    }

    Context 'with Priority' {
        AfterAll {
            Get-PackageProvider |
            ForEach-Object {
                $_.Priority = 100
            }
        }

        It 'should return <_> default priority' -ForEach 100 {
            Get-PackageProvider |
            Select-Object -ExpandProperty Priority -Unique |
            Should -Be $_
        }

        It 'should set <Provider> to <Priority> priority' -ForEach @{ Provider = 'PowerShell'; Priority = 50 } {
            $result = Get-PackageProvider -Name $Provider
            $result.Priority = $Priority

            Get-PackageProvider -Name $Provider |
            Select-Object -ExpandProperty Priority |
            Should -Be $Priority
        }
    }

    Context 'with pipeline input' {
        It 'should return <_> provider' -ForEach 'PowerShell', 'P*' {
            $_ |
            Get-PackageProvider |
            Should -Not -BeNullOrEmpty
        }
    }

    Context 'inside runspace' {
        BeforeAll {
            $scriptBlock = {
                Import-Module (Join-Path -Path $using:PSScriptRoot -ChildPath PowerShellProvider)
                Get-PackageProvider
            }
        }
        
        It 'should return results with Foreach-Object -Parallel' -ForEach 10 {
            1..$_ |
            ForEach-Object -UseNewRunspace -Parallel $scriptBlock |
            Should -HaveCount $_
        }

        It 'should return results with ThreadJob' -ForEach 10 {
            for ($i = 0; $i -lt $_; $i++) {
                Start-ThreadJob -ScriptBlock $scriptBlock
            }
            
            Get-Job |
            Wait-Job |
            Receive-Job |
            Should -HaveCount $_
        }
    }

    Context 'provider completer' {
        BeforeDiscovery {
            $completerTests = @(
                @{ Completion = 'P'; Provider = 'PowerShell'; Name = 'PowerShell'; Count = 1 }    
                @{ Completion = 'Space'; Provider = "'Space Provider'"; Name = 'Space Provider'; Count = 1 }
                @{ Completion = 'So'; Provider = @('TestProviderA\Soda', 'TestProviderB\Soda'); Name = @('TestProviderA\Soda', 'TestProviderB\Soda'); Count = 2 }
                @{ Completion = '*Provider'; Provider = @("'Space Provider'", 'PowerShell', 'TestProviderA\Soda', 'TestProviderB\Soda'); Name = @('Space Provider', 'PowerShell', 'TestProviderA\Soda', 'TestProviderB\Soda'); Count = 4 }
            )
        }
        
        It 'should return all imported providers' {
            TabExpansion2 'Get-PackageProvider ' |
            Select-Object -ExpandProperty CompletionMatches |
            Measure-Object |
            Select-Object -ExpandProperty Count |
            Should -BeExactly 4
        }

        It 'should return correct completion properties for <Provider>' -ForEach $completerTests {
            $results = TabExpansion2 "Get-PackageProvider -Name $($_.Completion)" |
            Select-Object -ExpandProperty CompletionMatches

            $results | 
            Select-Object -ExpandProperty CompletionText |
            Should -BeExactly $_.Provider

            $results |
            Select-Object -ExpandProperty ListItemText |
            Should -BeExactly $_.Name

            $results |
            Should -HaveCount $_.Count
        }
    }
}
