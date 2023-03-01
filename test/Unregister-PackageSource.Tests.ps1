#requires -modules AnyPackage

Describe Unregister-PackageSource {
    BeforeAll {
        Import-Module (Join-Path -Path $PSScriptRoot -ChildPath PowerShellProvider)

        $path = Join-Path -Path TestDrive: -ChildPath repo
        New-Item -Path $path -ItemType Directory
    }

    BeforeEach {
        $registerParams = @{
            Name = 'Test'
            Location = (Join-Path -Path TestDrive: -ChildPath repo)
            Provider = 'PowerShell'
        }

        Register-PackageSource @registerParams
    }

    AfterEach {
        $provider = Get-PackageProvider

        if ($provider.Sources.Count -gt 1) {
            $provider.Sources.RemoveAt($provider.Sources.Count - 1)
        }
    }

    Context 'with -Name parameter' {
        It 'should unregister <_> source' -ForEach 'Test' {
            Unregister-PackageSource -Name $_ -PassThru |
            Should -Not -BeNullOrEmpty
        }
    }

    Context 'with pipeline' {
        It 'should unregister <_> from Get-PackageSource' -ForEach 'Test' {
            Get-PackageSource -Name $_ |
            Unregister-PackageSource -PassThru |
            Should -Not -BeNullOrEmpty
        }

        It 'should unregister <_> from string' -ForEach 'Test' {
            $_ |
            Unregister-PackageSource -PassThru |
            Should -Not -BeNullOrEmpty
        }
    }
}