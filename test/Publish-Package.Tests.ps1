#requires -modules AnyPackage

Describe Publish-Package {
    BeforeAll {
        Import-Module (Join-Path -Path $PSScriptRoot -ChildPath PowerShellProvider)

        [PSCustomObject]@{
            Name = 'Apple'
            Version = '4.0'
            Description = 'Makes a good pie.'
        } |
        ConvertTo-Json |
        Out-File -FilePath (Join-Path -Path TestDrive: -ChildPath apple-4.0.json)
    }

    Context 'with -Path parameter' {
        AfterEach {
            $provider = Get-PackageProvider
            Remove-Item -Path (Join-Path -Path $provider.Sources.Location -ChildPath apple-4.0.json) -ErrorAction Ignore
        }

        It 'should publish package' {
            $publishParams = @{
                Path     = (Join-Path -Path TestDrive: -ChildPath apple-4.0.json)
                Provider = 'PowerShell'
            }

            Publish-Package @publishParams -PassThru |
            Should -Not -BeNullOrEmpty
        }
    }

    Context 'with -Source parameter' {
        BeforeAll {
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

        It 'should publish to <_> source' -ForEach 'Test' {
            $publishParams = @{
                Path     = (Join-Path -Path TestDrive: -ChildPath apple-4.0.json)
                Provider = 'PowerShell'
            }

            $results = Publish-Package @publishParams -Source $_ -PassThru

            $results | Should -Not -BeNullOrEmpty
            $results.Source.Name | Should -Be $_
        }
    }
}
