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
        public string[] Name { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets if available providers are returned. 
        /// </summary>
        [Parameter]
        public SwitchParameter ListAvailable { get; set; }

        private List<PackageProviderInfo> _availableProviders = new List<PackageProviderInfo>();

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
            if (Name.Length > 0)
            {
                List<PackageProviderInfo> provider;

                foreach (var name in Name)
                {
                    if (ListAvailable)
                    {
                        provider = _availableProviders.Where(x => x.IsMatch(name)).ToList();
                    }
                    else {
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
            else if (ListAvailable)
            {
                WriteAvailable(_availableProviders);
            }
            else
            {
                WriteObject(GetProviders(), true);
            }
        }

        private List<PackageProviderInfo> GetAvailableProviders()
        {
            var modules = PowerShell
                          .Create()
                          .AddCommand("Get-Module")
                          .AddParameter("ListAvailable")
                          .Invoke<PSModuleInfo>();

            List<PackageProviderInfo> providerInfos = new List<PackageProviderInfo>();

            foreach (var module in modules)
            {
                var privateData = module.PrivateData as Hashtable;

                if (privateData is null) { continue; }
                
                if (privateData.ContainsKey("AnyPackage"))
                {
                    var anyPackage = privateData["AnyPackage"] as Hashtable;

                    if (anyPackage is null) { continue; }

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
