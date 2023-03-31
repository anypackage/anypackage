// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
        public PackageVersion? Version { get; }

        /// <summary>
        /// Gets the package description.
        /// </summary>
        public string Description { get; } = string.Empty;

        /// <summary>
        /// Gets the package source.
        /// </summary>
        public PackageSourceInfo? Source { get; }

        /// <summary>
        /// Gets additional metadata about the package.
        /// </summary>
        public IReadOnlyDictionary<string, object> Metadata { get; }

        /// <summary>
        /// Gets the package provider.
        /// </summary>
        public PackageProviderInfo Provider { get; }

        /// <summary>
        /// Gets package dependencies.
        /// </summary>
        public IEnumerable<PackageDependency> Dependencies => _dependencies;

        private List<PackageDependency> _dependencies = new List<PackageDependency>();

        /// <summary>
        /// Instantiates a <c>PackageInfo</c> object.
        /// </summary>
        /// <param name="name">Package name.</param>
        /// <param name="version">Package version.</param>
        /// <param name="description">Package description.</param>
        /// <param name="providerInfo">Package provider info.</param>
        /// <param name="source">Package source.</param>
        /// <param name="metadata">Additional package metadata.</param>
        /// <param name="dependencies">Package dependencies.</param>
        internal PackageInfo(string name,
                             PackageVersion? version,
                             string? description,
                             PackageProviderInfo providerInfo,
                             PackageSourceInfo? source,
                             IDictionary<string, object>? metadata,
                             IEnumerable<PackageDependency>? dependencies) : this(name, version, description, providerInfo, source, dependencies)
        {
            if (metadata is not null)
            {
                Metadata = new ReadOnlyDictionary<string, object>(metadata);
            }
        }

        /// <summary>
        /// Instantiates a <c>PackageInfo</c> object.
        /// </summary>
        /// <remarks>
        /// Metadata hashtable keys will be converted to strings.
        /// </remarks>
        /// <param name="name">Package name.</param>
        /// <param name="version">Package version.</param>
        /// <param name="description">Package description.</param>
        /// <param name="providerInfo">Package provider info.</param>
        /// <param name="source">Package source.</param>
        /// <param name="metadata">Additional package metadata.</param>
        /// <param name="dependencies">Package dependencies.</param>
        internal PackageInfo(string name,
                             PackageVersion? version,
                             string? description,
                             PackageProviderInfo providerInfo,
                             PackageSourceInfo? source,
                             Hashtable? metadata,
                             IEnumerable<PackageDependency>? dependencies) : this(name, version, description, providerInfo, source, dependencies)
        {
            if (metadata is not null && metadata.Count > 0)
            {
                var dictionary = new Dictionary<string, object>();

                foreach (var key in metadata.Keys)
                {
                    dictionary.Add(key.ToString(), metadata[key]);
                }

                Metadata = new ReadOnlyDictionary<string, object>(dictionary);
            }
        }

        private PackageInfo(string name,
                            PackageVersion? version,
                            string? description,
                            PackageProviderInfo providerInfo,
                            PackageSourceInfo? source,
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
            Metadata = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>());

            if (description is not null)
            {
                Description = description;
            }

            if (dependencies is not null)
            {
                _dependencies = new List<PackageDependency>(dependencies);
            }
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
