#requires -modules AnyPackage

[Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseDeclaredVarsMoreThanAssignments',
    '',
    Justification = 'Does not work with Pester scopes.')]
param()

Describe Save-Package {
    BeforeAll {
        Import-Module (Join-Path -Path $PSScriptRoot -ChildPath PowerShellProvider)
        $savePath = (Resolve-Path -Path TestDrive:).ProviderPath
    }

    AfterEach {
        Remove-Item -Path TestDrive:/*.json
    }

    Context 'with -Name parameter' {
        It 'should save <_>' -ForEach 'Apple', 'Banana' {
            Save-Package -Name $_ -Path $savePath -PassThru |
            Should -Not -BeNullOrEmpty
        }

        It 'should write error for <_> non-existent package' -ForEach 'broke' {
            { Save-Package -Name $_ -Path $savePath -ErrorAction Stop } |
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

        It 'should save package for <_> version range' -ForEach $versions {
            Save-Package -Name Apple -Version $_ -Path $savePath -PassThru |
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

        It 'should save package from <_> source' -ForEach 'Test' {
            $results = Save-Package -Name Apple -Source $_ -Path $savePath -PassThru

            $results.Source |
            Select-Object -ExpandProperty Name -Unique |
            Should -Be $_
        }
    }

    Context 'with -Prerelease parameter' {
        It 'should save prerelease version' {
            Save-Package -Name Apple -Prerelease -Path $savePath -PassThru |
            Where-Object { $_.Version.IsPrerelease } |
            Should -Not -BeNullOrEmpty
        }
    }

    Context 'with pipeline' {
        It 'should save <_> package from Find-Package' -ForEach 'Apple', @('Apple', 'Banana') {
            $results = Find-Package -Name $_ |
            Group-Object -Property Name |
            ForEach-Object { $_.Group | Sort-Object -Property Version -Descending | Select-Object -First 1 } |
            Save-Package -Path $savePath -PassThru

            $results | Should -HaveCount @($_).Length
        }

        It 'should save <_> package from string' -ForEach 'Apple', @('Apple', 'Banana') {
            $results = $_ |
            Save-Package -Path $savePath -PassThru

            $results | Should -HaveCount @($_).Length
        }
    }
}
