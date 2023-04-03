// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;

namespace AnyPackage.Provider
{
    /// <summary>
    /// Indicates a package provider supports the <c>Path</c> parameter set
    /// and what file extensions are supported.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class PackageByFileAttribute : Attribute
    {
        /// <summary>
        /// Gets the file extension.
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Instantiates the <c>PackageByPathAttribute</c> attribute.
        /// </summary>
        /// <param name="extension">Supported file extension.</param>
        /// <exception cref="ArgumentNullException">If a value is null.</exception>
        /// <exception cref="ArgumentException">If a value is empty or whitespace.</exception>
        public PackageByFileAttribute(string extension)
        {
            if (extension is null)
            {
                throw new ArgumentNullException(nameof(extension));
            }

            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new ArgumentException("Cannot be empty or whitespace.", nameof(extension));
            }

            Extension = extension;
        }

        /// <inheritdoc />
        public override object TypeId => this;
    }
}
