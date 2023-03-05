// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using NuGet.Versioning;

namespace AnyPackage.Provider
{
    /// <summary>
    /// The <c>PackageInfo</c> class.
    /// </summary>
    public sealed class PackageInfo
    {
        /// <summary>
        /// Gets the package name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the package version.
        /// </summary>
        public NuGetVersion Version { get; }

        /// <summary>
        /// Gets the package description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the package source.
        /// </summary>
        public PackageSourceInfo? Source { get; }

        /// <summary>
        /// Gets additional metadata about the package.
        /// </summary>
        // TODO: Change to an IDictionary<string,object>
        public Hashtable Metadata { get; }

        /// <summary>
        /// Gets the package provider.
        /// </summary>
        public PackageProviderInfo Provider { get; }

        /// <summary>
        /// Gets package dependencies.
        /// </summary>
        public IEnumerable<PackageDependency> Dependencies => _dependencies;

        private List<PackageDependency> _dependencies;

        /// <summary>
        /// Instantiates 
        /// </summary>
        /// <param name="name">Package name.</param>
        /// <param name="version">Package version.</param>
        /// <param name="description">Package description.</param>
        /// <param name="providerInfo">Package provider info.</param>
        /// <param name="source">Package source.</param>
        /// <param name="metadata">Additional package metadata.</param>
        /// <param name="dependencies">Package dependencies.</param>
        internal PackageInfo(string name,
                           NuGetVersion version,
                           string description,
                           PackageProviderInfo providerInfo,
                           PackageSourceInfo? source,
                           Hashtable? metadata,
                           IEnumerable<PackageDependency>? dependencies)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
            }
            
            Name = name;
            Version = version;
            Source = source;
            Provider = providerInfo;
            Description = description ?? string.Empty;
            Metadata = metadata ?? new Hashtable();
            _dependencies = dependencies is not null ? new List<PackageDependency>(dependencies) : new List<PackageDependency>();
        }

        /// <summary>
        /// Returns a string of the package name.
        /// </summary>
        /// <returns>
        /// The package name.
        /// </returns>
        public override string ToString() => Name;

        // TODO: Add set property methods to return new object with updated values.
    }
}
