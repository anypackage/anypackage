#requires -modules AnyPackage

[Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseDeclaredVarsMoreThanAssignments',
    '',
    Justification = 'Does not work with Pester scopes.')]
param()

Describe Uninstall-Package {
    BeforeAll {
        Import-Module (Join-Path -Path $PSScriptRoot -ChildPath PowerShellProvider)
    }

    AfterEach {
        $provider = Get-PackageProvider -Name PowerShell
        if ($provider.Packages.Count -gt 0) {
            $provider.Packages.Clear()
        }
    }

    Context 'with -Name parameter' {
        BeforeEach {
            Install-Package -Name Apple, Banana
        }
        
        It 'should uninstall <_> package' -ForEach 'Apple', @('Apple', 'Banana') {
            $results = Uninstall-Package -Name $_ -PassThru

            $results | Should -Not -BeNullOrEmpty
            $results | Should -HaveCount @($_).Length
        }

        It 'should fail to uninstall <_> package' -ForEach 'broke' {
            { Uninstall-Package -Name $_ -ErrorAction Stop } |
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

        BeforeEach {
            Find-Package -Name Apple |
            Install-Package
        }

        It 'should uninstall package for <_> version range' -ForEach $versions {
            Uninstall-Package -Name Apple -Version $_ -PassThru |
            Should -Not -BeNullOrEmpty
        }
    }   
}
