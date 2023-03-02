// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Management.Automation;
using NuGet.Versioning;

namespace AnyPackage.Commands
{
    /// <summary>
    /// The NuGet version range parameter validator.
    /// </summary>
    public sealed class VersionRangeTransformationAttribute : ArgumentTransformationAttribute
    {
        /// <see href="link">https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.argumenttransformationattribute.transform</see>
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            if (inputData is null)
            {
                throw new ArgumentTransformationMetadataException("The argument is null. Provide a valid version range, and then try running the command again.");
            }
            else if (inputData is VersionRange)
            {
                return inputData;
            }
            else
            {
                var input = inputData.ToString();

                // If a minimum inclusive string like 1.0 is passed
                // convert it an exact version match. This is done
                // because the user expects the verbatim version to be used.
                // If the user wishes to use minimum inclusive they
                // will need to use the full syntax of [1.0,]
                if (!input.Contains("*") &&
                    !input.Contains("(") &&
                    !input.Contains("["))
                {
                    input = $"[{input}]";
                }

                try
                {
                    return VersionRange.Parse(input);
                }
                catch (Exception ex)
                {
                    throw new ArgumentTransformationMetadataException($"'{inputData}' is not a valid version range.", ex);
                }
            }
        }
    }
}
