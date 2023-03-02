// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Collections;
using System.Linq;
using System.Management.Automation;

namespace AnyPackage.Commands
{
    /// <summary>
    /// The no wildcards parameter validator.
    /// </summary>
    public sealed class ValidateNoWildcardsAttribute : ValidateArgumentsAttribute
    {
        /// <see href="link">https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.validateargumentsattribute.validate</see>
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            var collection = arguments as IEnumerable;

            if (collection is not null)
            {
                var values = collection
                             .Cast<object>()
                             .Select(x => x.ToString())
                             .ToArray();

                foreach (var value in values)
                {
                    if (WildcardPattern.ContainsWildcardCharacters(value))
                    {
                        throw new ValidationMetadataException("The parameter does not support wildcards.");
                    }
                }
            }
        }
    }
}
