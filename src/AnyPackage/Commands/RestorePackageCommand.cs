// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Collections;
using System.Management.Automation;

using AnyPackage.Provider;

namespace AnyPackage.Commands;

/// <summary>
/// The Restore-Package command.
/// </summary>
[Cmdlet(VerbsData.Restore, "Package",
        SupportsShouldProcess = true,
        HelpUri = "https://go.anypackage.dev/Restore-Package")]
[OutputType(typeof(PackageInfo))]
public sealed class RestorePackageCommand : PSCmdlet
{
    /// <summary>
    /// Gets or sets required file path.
    /// </summary>
    [Parameter()]
    [ValidateNotNullOrEmpty]
    public string RequiredFile { get; set; } = string.Empty;

    /// <summary>
    /// Processes input.
    /// </summary>
    protected override void EndProcessing()
    {
        foreach (var context in ParseRequiredFile())
        {
            var installed = GetPackage(context);

            if (installed is not null && context.Latest && !IsLatest(context, installed))
            {
                // TODO: Support updating using Install-Package
                // when a provider doesn't support Update-Package
                installed = UpdatePackage(context);
            }
            else if (installed is null)
            {
                installed = InstallPackage(context);
            }

            if (installed is not null)
            {
                WriteObject(installed);
            }
        }
    }

    private PackageInfo? GetPackage(RestoreContext context)
    {
        using var powershell = PowerShell.Create(RunspaceMode.CurrentRunspace);
        powershell.AddCommand("Get-Package")
                  .AddParameter("Name", context.Name)
                  .AddParameter("Provider", context.Provider);

        if (context.Version is not null)
        {
            powershell.AddParameter("Version", context.Version);
        }

        return powershell.Invoke<PackageInfo>()
                         .OrderByDescending(x => x.Version)
                         .FirstOrDefault();
    }

    private PackageInfo InstallPackage(RestoreContext context)
    {
        using var powershell = PowerShell.Create(RunspaceMode.CurrentRunspace);
        powershell.AddCommand("Install-Package")
                  .AddParameter("Name", context.Name)
                  .AddParameter("Provider", context.Provider)
                  .AddParameter("PassThru");

        if (context.Version is not null)
        {
            powershell.AddParameter("Version", context.Version);
        }

        if (context.Source is not null)
        {
            powershell.AddParameter("Source", context.Source);
        }

        if (context.Prerelease)
        {
            powershell.AddParameter("Prerelease");
        }

        powershell.AddCommand("Where-Object")
                  .AddParameter("Property", "Name")
                  .AddParameter("eq")
                  .AddParameter("Value", context.Name);

        return powershell.Invoke<PackageInfo>().First();
    }

    private PackageInfo UpdatePackage(RestoreContext context)
    {
        using var powershell = PowerShell.Create(RunspaceMode.CurrentRunspace);
        powershell.AddCommand("Update-Package")
                  .AddParameter("Name", context.Name)
                  .AddParameter("Provider", context.Provider)
                  .AddParameter("PassThru");

        if (context.Version is not null)
        {
            powershell.AddParameter("Version", context.Version);
        }

        if (context.Source is not null)
        {
            powershell.AddParameter("Source", context.Source);
        }

        if (context.Prerelease)
        {
            powershell.AddParameter("Prerelease");
        }

        powershell.AddCommand("Where-Object")
                  .AddParameter("Property", "Name")
                  .AddParameter("eq")
                  .AddParameter("Value", context.Name);

        return powershell.Invoke<PackageInfo>().First();
    }

    private bool IsLatest(RestoreContext context, PackageInfo installed)
    {
        if (installed.Version is null)
        {
            throw new InvalidOperationException($"Installed package '{context.Name}' does not have a version.");
        }

        using var powershell = PowerShell.Create(RunspaceMode.CurrentRunspace);
        powershell.AddCommand("Find-Package")
                  .AddParameter("Name", context.Name)
                  .AddParameter("Provider", context.Provider);

        if (context.Version is not null)
        {
            powershell.AddParameter("Version", context.Version);
        }

        if (context.Source is not null)
        {
            powershell.AddParameter("Source", context.Source);
        }

        if (context.Prerelease)
        {
            powershell.AddParameter("Prerelease");
        }

        var latest = powershell.Invoke<PackageInfo>()
                               .OrderByDescending(x => x.Version)
                               .First();

        if (latest.Version is null)
        {
            throw new InvalidOperationException($"Available package '{context.Name}' does not have a version.");
        }

        return installed.Version >= latest.Version;
    }

    private IEnumerable<RestoreContext> ParseRequiredFile()
    {
        using var powershell = PowerShell.Create(RunspaceMode.CurrentRunspace);
        var hashtable = powershell.AddCommand("Import-PowerShellDataFile")
                                  .AddParameter("Path", RequiredFile)
                                  .Invoke<Hashtable>()
                                  .First();

        foreach (var provider in hashtable.Keys)
        {
            IEnumerable? packages = null;

            if (hashtable[provider] is Array array)
            {
                packages = array;
            }

            if (packages is null) { continue; }

            foreach (var package in packages)
            {
                if (package is string name)
                {
                    yield return new RestoreContext()
                    {
                        Name = name,
                        Provider = provider.ToString()
                    };
                }
                else if (package is Hashtable ht)
                {
                    var request = new RestoreContext()
                    {
                        Name = ht["Name"].ToString(),
                        Provider = provider.ToString()
                    };

                    if (ht.ContainsKey("Version"))
                    {
                        request.Version = PackageVersionRange.Parse(ht["Version"].ToString());
                    }

                    if (ht.ContainsKey("Source"))
                    {
                        request.Source = ht["Source"].ToString();
                    }

                    if (ht.ContainsKey("Latest"))
                    {
                        request.Latest = bool.Parse(ht["Latest"].ToString());
                    }

                    if (ht.ContainsKey("Prerelease"))
                    {
                        request.Prerelease = bool.Parse(ht["Prerelease"].ToString());
                    }

                    yield return request;
                }
            }
        }
    }

    // TODO: Move to separate file
    internal class RestoreContext
    {
        internal string Name { get; set; } = string.Empty;
        internal PackageVersionRange? Version { get; set; }
        internal string? Source { get; set; }
        internal bool Latest { get; set; }
        internal bool Prerelease { get; set; }
        internal string Provider { get; set; } = string.Empty;
    }
}
