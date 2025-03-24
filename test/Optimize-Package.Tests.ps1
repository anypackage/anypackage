#requires -modules AnyPackage

[Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseDeclaredVarsMoreThanAssignments',
    '',
    Justification = 'Does not work with Pester scopes.')]
param()

Describe Optimize-Package {
    BeforeAll {
        Import-Module (Join-Path -Path $PSScriptRoot -ChildPath PowerShellProvider)
    }

    BeforeEach {
        Install-Package -Name Apple, Banana -Version 1.0
        Install-Package -Name Apple, Banana -Version 2.0
    }

    AfterEach {
        Uninstall-Package -Name Apple, Banana -ErrorAction SilentlyContinue
    }

    Context 'with no additional parameters' {
        It 'should return packages' {
            Optimize-Package -PassThru |
            Should -HaveCount 2
        }
    }

    Context 'with -Name parameter' {
        BeforeDiscovery {
            $names = @('Apple', 'Banana', @('Apple', 'Banana'), 'A*')
        }

        It 'should return <_> package' -ForEach $names {
            Optimize-Package -Name $_ -PassThru |
            Should -Not -BeNullOrEmpty
        }

        It 'should fail with <_> non-existent package' -ForEach 'broke' {
            { Optimize-Package -Name $_ -PassThru -ErrorAction Stop } |
            Should -Throw -ExpectedMessage "Package not found. (Package '$_')"
        }
    }

    Context 'with pipeline input' {
        It 'should return packages for <_>' -ForEach 'Apple', 'Banana', 'A*' {
            $_ |
            Optimize-Package -PassThru |
            Should -Not -BeNullOrEmpty
        }
    }
}
