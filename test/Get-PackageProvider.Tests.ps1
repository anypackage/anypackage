#requires -modules AnyPackage

Describe Get-PackageProvider {
    BeforeAll {
        Import-Module (Join-Path -Path $PSScriptRoot -ChildPath PowerShellProvider)
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

        It 'should have correct properties for <Provider> provider' -ForEach @{ Provider = 'PowerShell'; Type = 'PowerShellProvider'; Module = 'PowerShellProvider'; Operations = 257 } {
            $result = Get-PackageProvider -Name $Provider

            $result.Name | Should -Be $Provider
            $result.Id | Should -Not -Be [guid]::Empty
            $result.Module.Name | Should -Be $Module
            $result.ModuleName | Should -Be $Module
            $result.FullName | Should -Be "$Module\$Provider"
            $result.Operations | Should -Be $Operations
            $result.ImplementingType.Name | Should -Be $Type
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
            Select-Object -ExpandProperty Priority |
            Should -Be 100
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
}
