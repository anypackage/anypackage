// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using UniversalPackageManager.Provider;
using static UniversalPackageManager.Provider.PackageProviderManager;

namespace UniversalPackageManager.Commands
{
    [Cmdlet(VerbsCommon.Get, "PackageProvider")]
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

        protected override void ProcessRecord()
        {
            if (Name.Length > 0)
            {
                List<PackageProviderInfo> provider;
                
                foreach (var name in Name)
                {
                    provider = GetProviders(name).ToList();

                    if (provider.Count > 0)
                    {
                        WriteObject(provider, true);
                    }
                    else
                    {
                        var e = new PackageProviderNotFoundException(name);
                        var err = new ErrorRecord(e, "PackageProviderNotFoundError", ErrorCategory.ObjectNotFound, name);
                        WriteError(err);
                    }
                }
            }
            else
            {
                WriteObject(GetProviders(), true);
            }
        }
    }
}
