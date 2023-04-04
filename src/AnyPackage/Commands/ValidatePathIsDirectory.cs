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
            string path;

            try
            {
                path = engineIntrinsics.SessionState.Path.GetUnresolvedProviderPathFromPSPath((string)arguments);
            }
            catch (Exception e)
            {
                throw new ValidationMetadataException(e.Message, e);
            }

            if (File.Exists(path))
            {
                var ex = new InvalidOperationException($"Path '{path}' is a file.");
                throw new ValidationMetadataException(ex.Message, ex);
            }
            else if (!Directory.Exists(path))
            {
                var ex = new DirectoryNotFoundException($"Path '{path}' does not exist.");
                throw new ValidationMetadataException(ex.Message, ex);
            }
        }
    }
}
