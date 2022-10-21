// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Linq;
using System.Management.Automation;
using UniversalPackageManager.Provider;
using static UniversalPackageManager.Provider.PackageProviderManager;

namespace UniversalPackageManager.Commands
{
    public sealed class ValidateProviderAttribute : ValidateArgumentsAttribute
    {
        private PackageProviderOperations _operations;

        public ValidateProviderAttribute(PackageProviderOperations operations)
        {
            _operations = operations;
        }

        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            var provider = arguments.ToString();

            if (WildcardPattern.ContainsWildcardCharacters(provider))
            {
                throw new ValidationMetadataException("Wildcard characters not supported.");
            }

            var providers = GetProviders(provider, _operations).ToArray();

            if (providers.Length > 1)
            {
                throw new ValidationMetadataException($"Multiple package providers found for '{provider}'. Use provider full name 'Module\\Provider' instead.");
            }
            else if (providers.Length == 0)
            {
                throw new ValidationMetadataException($"Package provider '{provider}' does not support this operation or cannot be found. Use 'Get-PackageProvider -Name {provider}' to see if it is present and supports this operation.");
            }
        }
    }
}
