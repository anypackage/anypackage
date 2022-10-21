// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;

namespace UniversalPackageManager.Provider
{
    /// <summary>
    /// Identifies a class as a package provider.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PackageProviderAttribute : Attribute
    {
        /// <summary>
        /// Gets the provider name.
        /// </summary>
        public string Name { get; }

        private char[] _invalidCharacters = new char[] { ':', '\\', '[', ']', '?', '*' };

        /// <summary>
        /// Constructor for the package provider attribute.
        /// </summary>
        /// <param name="name">The provider name.</param>
        public PackageProviderAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The provider name cannot be null or whitespace.", nameof(name));
            }

            if (name.IndexOfAny(_invalidCharacters) != -1)
            {
                throw new ArgumentOutOfRangeException(nameof(name), "Contains invalid characters.");
            }

            Name = name;
        }
    }
}
