#requires -modules AnyPackage

[Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseDeclaredVarsMoreThanAssignments',
    '',
    Justification = 'Does not work with Pester scopes.')]
param()

Describe Get-Package {
    BeforeAll {
        Import-Module (Join-Path -Path $PSScriptRoot -ChildPath PowerShellProvider)

        $provider = Get-PackageProvider
        $paths = Get-ChildItem -Path $provider.Sources[0].Location

        foreach ($path in $paths) {
            $provider.Packages += Get-Content -Path $path |
            ConvertFrom-Json
        }
    }

    AfterAll {
        $provider.Packages.Clear()
    }

    Context 'with no additional parameters' {
        It 'should return packages' {
            Get-Package |
            Should -Not -BeNullOrEmpty
        }
    }

    Context 'with -Name parameter' {
        BeforeDiscovery {
            $names = @('Apple', 'Banana', @('Apple', 'Banana'), 'A*')
        }

        It 'should return <_> package' -ForEach $names {
            Get-Package -Name $_ |
            Should -Not -BeNullOrEmpty
        }

        It 'should fail with <_> non-existent package' -ForEach 'broke' {
            { Get-Package -Name $_ -ErrorAction Stop } |
            Should -Throw -ExpectedMessage "Package not found. (Package '$_')"
        }
    }

    Context 'with -Version parameter' {
        BeforeDiscovery {
            $versions = @(
                '1.0',
                '1.0',
                '[1.0,]',
                '(1.0,)',
                '(,2.0)',
                '(1.0,2.0]',
                '(1.0,2.0)',
                '[1.0,2.0)'
            )
        }

        It 'should return packages for <_> version range' -ForEach $versions {
            Get-Package -Version $_ |
            Should -Not -BeNullOrEmpty
        }
    }

    Context 'with pipeline input' {
        It 'should return packages for <_>' -ForEach 'Apple', 'Banana', 'A*' {
            $_ |
            Get-Package |
            Should -Not -BeNullOrEmpty
        }
    }
}
