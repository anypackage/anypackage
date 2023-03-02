// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.IO;
using System.Management.Automation;

namespace AnyPackage.Commands
{
    /// <summary>
    /// The path is directory parameter validator.
    /// </summary>
    public sealed class ValidatePathIsDirectoryAttribute : ValidateArgumentsAttribute
    {
        /// <see href="link">https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.validateargumentsattribute.validate</see>
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
            if (File.Exists(arguments.ToString()))
            {
                var ex = new InvalidOperationException($"The path '{arguments}' is a file.");
                throw new ValidationMetadataException(ex.Message, ex);
            }
            else if (!Directory.Exists(arguments.ToString()))
            {
                var ex = new DirectoryNotFoundException($"The path '{arguments}' does not exist.");
                throw new ValidationMetadataException(ex.Message, ex);
            }
        }
    }
}
