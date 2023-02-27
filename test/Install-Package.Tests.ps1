#requires -modules AnyPackage

[Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseDeclaredVarsMoreThanAssignments',
    '',
    Justification = 'Does not work with Pester scopes.')]
param()

Describe Install-Package {
    BeforeAll {
        Import-Module (Join-Path -Path $PSScriptRoot -ChildPath PowerShellProvider)
    }

    AfterEach {
        $provider = Get-PackageProvider
        $provider.Packages.Clear()
    }

    Context 'with -Name parameter' {
        It 'should install <_> package' -ForEach 'Apple', @('Apple', 'Banana') {
            $results = Install-Package -Name $_ -PassThru
    
            $results | Should -Not -BeNullOrEmpty
            $results | Should -HaveCount @($_).Length
        }

        It 'should fail to install <_> package' -ForEach 'broke' {
            { Install-Package -Name $_ -ErrorAction Stop } |
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

        It 'should install package for <_> version range' -ForEach $versions {
            Install-Package -Name Apple -Version $_ -PassThru |
            Should -Not -BeNullOrEmpty
        }
    }

    Context 'with -Source parameter' {
        BeforeAll {
            $path = Join-Path -Path TestDrive: -ChildPath repo
            New-Item -Path $path -ItemType Directory
            $provider = Get-PackageProvider
            $provider.Sources.Add([PSCustomObject]@{ Name = 'Test'; Location = $path; Trusted = $true })
            Copy-Item -Path $PSScriptRoot/packages/* -Destination $path
        }

        AfterAll {
            if ($provider.Sources.Count -gt 1) {
                $provider.Sources.RemoveAt($provider.Sources.Count - 1)
            }
        }

        It 'should install package from <_> source' -ForEach 'Test' {
            $results = Install-Package -Name Apple -Source $_ -PassThru

            $results.Source |
            Select-Object -ExpandProperty Name -Unique |
            Should -Be $_
        }
    }

    Context 'with pipeline' {
        It 'should install <_> package from Find-Package' -TestCases 'Apple', @('Apple', 'Banana') {
            $results = Find-Package -Name $_ |
            Group-Object -Property Name |
            ForEach-Object { $_.Group | Sort-Object -Property Version -Descending | Select-Object -First 1 } |
            Install-Package -PassThru

            $results | Should -HaveCount @($_).Length
        }

        It 'should install <_> package from string' -TestCases 'Apple', @('Apple', 'Banana') {
            $results = $_ |
            Install-Package -PassThru

            $results | Should -HaveCount @($_).Length
        }
    }
}
