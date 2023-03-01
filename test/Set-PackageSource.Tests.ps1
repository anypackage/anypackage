#requires -modules AnyPackage

Describe Set-PackageSource {
    BeforeAll {
        Import-Module (Join-Path -Path $PSScriptRoot -ChildPath PowerShellProvider)

        $path = Join-Path -Path TestDrive: -ChildPath repo
        New-Item -Path $path -ItemType Directory
        $provider = Get-PackageProvider
        $provider.Sources.Add([PSCustomObject]@{ Name = 'Test'; Location = $path; Trusted = $true })
    }

    AfterAll {
        if ($provider.Sources.Count -gt 1) {
            $provider.Sources.RemoveAt($provider.Sources.Count - 1)
        }
    }

    Context 'with -Location parameter' {
        It 'should set location' -ForEach 'Temp:' {
            $results = Set-PackageSource -Name Test -Location $_ -PassThru 

            $results | Should -Not -BeNullOrEmpty
            $results.Location | Should -Be $_
        }
    }

    Context 'with -Trusted parameter' {
        It 'should be trusted' {
            $results = Set-PackageSource -Name Test -Trusted -PassThru 

            $results | Should -Not -BeNullOrEmpty
            $results.Trusted | Should -BeTrue
        }

        It 'should not be trusted' {
            $results = Set-PackageSource -Name Test -Trusted:$false -PassThru 

            $results | Should -Not -BeNullOrEmpty
            $results.Trusted | Should -BeFalse
        }
    }

    Context 'with pipeline' {
        It 'should unregister <_> from Get-PackageSource' -ForEach 'Test' {
            Get-PackageSource -Name $_ |
            Set-PackageSource -Location TestDrive: -PassThru |
            Should -Not -BeNullOrEmpty
        }

        It 'should unregister <_> from string' -ForEach 'Test' {
            $_ |
            Set-PackageSource -Location TestDrive: -PassThru |
            Should -Not -BeNullOrEmpty
        }
    }
}
