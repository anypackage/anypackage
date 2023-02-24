// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using NuGet.Versioning;

namespace AnyPackage.Provider
{
    /// <summary>
    /// The <c>PackageDependency</c> class.
    /// Contains information about package dependency requirements.
    /// </summary>
    public sealed class PackageDependency
    {
        /// <summary>
        /// Gets the package name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the version range.
        /// </summary>
        public VersionRange VersionRange { get; } = VersionRange.AllStable;

        /// <summary>
        /// Constructs a new instance of the <c>PackageDependency</c> class.
        /// </summary>
        /// <param name="name">Package name.</param>
        public PackageDependency(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Package name cannot be null or whitespace.", nameof(name));
            }
            
            Name = name;
        }

        /// <summary>
        /// Constructs a new instance of the <c>PackageDependency</c> class.
        /// </summary>
        /// <param name="name">Package name.</param>
        /// <param name="versionRange">Version range.</param>
        public PackageDependency(string name, VersionRange versionRange) : this(name)
        {
            if (versionRange is null)
            {
                throw new ArgumentNullException(nameof(versionRange));
            }

            VersionRange = versionRange;
        }

        public override string ToString() => Name;
    }
}
