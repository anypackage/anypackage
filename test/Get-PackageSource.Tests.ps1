#requires -modules AnyPackage

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
}
