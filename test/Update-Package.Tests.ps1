#requires -modules AnyPackage

[Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseDeclaredVarsMoreThanAssignments',
    '',
    Justification = 'Does not work with Pester scopes.')]
param()

Describe Update-Package {
    BeforeAll {
        Import-Module (Join-Path -Path $PSScriptRoot -ChildPath PowerShellProvider)
    }

    BeforeEach {
        Install-Package -Name Apple, Banana -Version 1.0
    }

    AfterEach {
        Uninstall-Package -Name Apple, Banana
    }

    Context 'with no additional parameters' {
        It 'should update' {
            Update-Package -PassThru |
            Should -Not -BeNullOrEmpty
        }
    }

    Context 'with -Name parameter' {
        It 'should update <_> package' -ForEach 'Apple', @('Apple', 'Banana') {
            Update-Package -Name $_ -PassThru |
            Should -HaveCount @($_).Count
        }

        It 'should fail to update <_> package' -ForEach 'broke' {
            { Update-Package -Name $_ -ErrorAction Stop } |
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

        It 'should update package for <_> version range' -ForEach $versions {
            Update-Package -Name Apple -Version $_ -PassThru |
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

        It 'should update package from <_> source' -ForEach 'Test' {
            $results = Update-Package -Name Apple -Source $_ -PassThru

            $results.Source |
            Select-Object -ExpandProperty Name -Unique |
            Should -Be $_
        }
    }

    Context 'with -Prerelease parameter' {
        It 'should update prerelease version' {
            Update-Package -Name Apple -Prerelease -PassThru |
            Where-Object { $_.Version.IsPrerelease } |
            Should -Not -BeNullOrEmpty
        }
    }

    Context 'with pipeline' {
        It 'should update <_> package from Find-Package' -ForEach 'Apple', @('Apple', 'Banana') {
            $results = Find-Package -Name $_ |
            Group-Object -Property Name |
            ForEach-Object { $_.Group | Sort-Object -Property Version -Descending | Select-Object -First 1 } |
            Update-Package -PassThru

            $results | Should -HaveCount @($_).Length
        }

        It 'should update <_> package from string' -ForEach 'Apple', @('Apple', 'Banana') {
            $_ |
            Update-Package -PassThru |
            Should -HaveCount @($_).Length
        }
    }
}
