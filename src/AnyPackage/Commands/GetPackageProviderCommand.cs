// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using AnyPackage.Provider;
using static AnyPackage.Provider.PackageProviderManager;

namespace AnyPackage.Commands
{
    /// <summary>
    /// The Get-PackageProvider command.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "PackageProvider", HelpUri = "https://go.anypackage.dev/Get-PackageProvider")]
    [OutputType(typeof(PackageProviderInfo))]
    public sealed class GetPackageProviderCommand : PSCmdlet
    {
        /// <summary>
        /// Gets or sets the provider name(s).
        /// </summary>
        [Parameter(Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [SupportsWildcards]
        [ValidateNotNullOrEmpty]
        [ArgumentCompleter(typeof(ProviderArgumentCompleter))]
        [Alias("Provider")]
        public string[] Name { get; set; } = ["*"];

        /// <summary>
        /// Gets or sets if available providers are returned. 
        /// </summary>
        [Parameter]
        public SwitchParameter ListAvailable { get; set; }

        private List<PackageProviderInfo> _availableProviders = [];

        /// <summary>
        /// Initializes the command.
        /// </summary>
        protected override void BeginProcessing()
        {
            if (ListAvailable)
            {
                _availableProviders = GetAvailableProviders();
            }
        }

        /// <summary>
        /// Processes input.
        /// </summary>
        protected override void ProcessRecord()
        {
            foreach (var name in Name)
            {
                List<PackageProviderInfo> provider;
                
                if (ListAvailable)
                {
                    provider = _availableProviders.Where(x => x.IsMatch(name)).ToList();
                }
                else
                {
                    provider = GetProviders(name).ToList();
                }

                if (provider.Count > 0)
                {
                    if (ListAvailable)
                    {
                        WriteAvailable(provider);
                    }
                    else
                    {
                        WriteObject(provider, true);
                    }
                }
                else if (!WildcardPattern.ContainsWildcardCharacters(name))
                {
                    var e = new PackageProviderNotFoundException(name);
                    var err = new ErrorRecord(e, "PackageProviderNotFoundError", ErrorCategory.ObjectNotFound, name);
                    WriteError(err);
                }
            }
        }

        private List<PackageProviderInfo> GetAvailableProviders()
        {
            var modules = PowerShell
                          .Create(RunspaceMode.CurrentRunspace)
                          .AddCommand("Get-Module")
                          .AddParameter("ListAvailable")
                          .Invoke<PSModuleInfo>();

            List<PackageProviderInfo> providerInfos = [];

            foreach (var module in modules)
            {
                if (module.PrivateData is not Hashtable privateData) { continue; }

                if (privateData.ContainsKey("AnyPackage"))
                {
                    if (privateData["AnyPackage"] is not Hashtable anyPackage) { continue; }

                    if (anyPackage.ContainsKey("Providers"))
                    {
                        IEnumerable? providers = null;

                        if (anyPackage["Providers"] is string)
                        {
                            providers = new object[] { anyPackage["Providers"] };
                        }
                        else if (anyPackage["Providers"] is Array)
                        {
                            providers = anyPackage["Providers"] as Array;
                        }

                        if (providers is null) { continue; }

                        foreach (var provider in providers)
                        {
                            providerInfos.Add(new PackageProviderInfo(provider.ToString(), module));
                        }
                    }
                }
            }

            return providerInfos;
        }

        private void WriteAvailable(IEnumerable<PackageProviderInfo> providers)
        {
            foreach (var provider in providers)
            {
                var info = new PSObject(provider);
                info.TypeNames.Insert(0, "PackageProviderInfoGrouping");
                WriteObject(info);
            }
        }
    }
}
