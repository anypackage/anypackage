#requires -modules AnyPackage

Describe Register-PackageSource {
    BeforeAll {
        Import-Module (Join-Path -Path $PSScriptRoot -ChildPath PowerShellProvider)

        $path = Join-Path -Path TestDrive: -ChildPath repo
        New-Item -Path $path -ItemType Directory
    }

    AfterEach {
        $provider = Get-PackageProvider
        
        if ($provider.Sources.Count -gt 1) {
            $provider.Sources.RemoveAt($provider.Sources.Count - 1)
        }
    }

    Context 'with -Location parameter' {
        It 'should register for <_> provider' -ForEach 'PowerShell' {
            $registerParams = @{
                Name = 'Test'
                Location = (Join-Path -Path TestDrive: -ChildPath repo)
                Provider = $_
                PassThru = $true
            }

            $results = Register-PackageSource @registerParams

            $results | Should -Not -BeNullOrEmpty
            $results.Name | Should -Be $registerParams.Name
            $results.Location | Should -Be $registerParams.Location
            $results.Trusted | Should -BeFalse
        }
    }

    Context 'with -Trusted parameter' {
        It 'should register for <_> provider' -ForEach 'PowerShell' {
            $registerParams = @{
                Name = 'Test'
                Location = (Join-Path -Path TestDrive: -ChildPath repo)
                Provider = $_
                PassThru = $true
                Trusted = $true
            }

            Register-PackageSource @registerParams |
            Select-Object -ExpandProperty Trusted |
            Should -BeTrue
        }
    }
}
