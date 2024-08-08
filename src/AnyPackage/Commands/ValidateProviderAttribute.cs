// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Linq;
using System.Management.Automation;
using AnyPackage.Provider;
using AnyPackage.Resources;
using static AnyPackage.Provider.PackageProviderManager;

namespace AnyPackage.Commands
{
    /// <summary>
    /// The provider parameter validator.
    /// </summary>
    /// <remarks>
    /// Instantiates the <c>ValidateProviderAttribute</c> class.
    /// </remarks>
    /// <param name="operations">Specifies the package provider operation.</param>
    public sealed class ValidateProviderAttribute(PackageProviderOperations operations) : ValidateArgumentsAttribute
    {
        /// <see href="link">https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.validateargumentsattribute.validate</see>
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            var provider = arguments.ToString();
            if (provider is null) { return; }

            if (WildcardPattern.ContainsWildcardCharacters(provider))
            {
                throw new ValidationMetadataException(Strings.WildcardsNotSupported);
            }

            var providers = GetProviders(provider, operations).ToArray();

            if (providers.Length > 1)
            {
                throw new ValidationMetadataException(string.Format(Strings.MultiplePackageProvidersFound, provider));
            }
            else if (providers.Length == 0)
            {
                throw new ValidationMetadataException(string.Format(Strings.PackageProviderNotFoundOrSupportOperation, provider));
            }
        }
    }
}
