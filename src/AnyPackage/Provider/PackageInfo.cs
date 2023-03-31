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
        /// <param name="provider">Package provider info.</param>
        public PackageInfo(string name, PackageProviderInfo provider)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (provider is null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            Name = name;
            Provider = provider;
            Metadata = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>());
        }

        /// <summary>
        /// Instantiates a <c>PackageInfo</c> object.
        /// </summary>
        /// <param name="name">Package name.</param>
        /// <param name="version">Package version.</param>
        /// <param name="provider">Package provider info.</param>
        public PackageInfo(string name, PackageVersion? version, PackageProviderInfo provider)
            : this(name, provider)
        {
            Version = version;
        }

        /// <summary>
        /// Instantiates a <c>PackageInfo</c> object.
        /// </summary>
        /// <param name="name">Package name.</param>
        /// <param name="version">Package version.</param>
        /// <param name="description">Package description.</param>
        /// <param name="provider">Package provider info.</param>
        public PackageInfo(string name, PackageVersion? version, string description, PackageProviderInfo provider)
            : this(name, version, provider)
        {
            if (description is null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            Description = description; 
        }

        /// <summary>
        /// Instantiates a <c>PackageInfo</c> object.
        /// </summary>
        /// <param name="name">Package name.</param>
        /// <param name="version">Package version.</param>
        /// <param name="source">Package source.</param>
        /// <param name="provider">Package provider info.</param>
        public PackageInfo(string name, PackageVersion? version, PackageSourceInfo source, PackageProviderInfo provider)
            : this(name, version, provider)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            Source = source;
        }

        /// <summary>
        /// Instantiates a <c>PackageInfo</c> object.
        /// </summary>
        /// <param name="name">Package name.</param>
        /// <param name="version">Package version.</param>
        /// <param name="source">Package source.</param>
        /// <param name="description">Package description.</param>
        /// <param name="provider">Package provider info.</param>
        public PackageInfo(string name,
                           PackageVersion? version,
                           PackageSourceInfo? source,
                           string description,
                           PackageProviderInfo provider)
            : this(name, version, description, provider)
        {
            Source = source;
        }

        /// <summary>
        /// Instantiates a <c>PackageInfo</c> object.
        /// </summary>
        /// <param name="name">Package name.</param>
        /// <param name="version">Package version.</param>
        /// <param name="source">Package source.</param>
        /// <param name="description">Package description.</param>
        /// <param name="dependencies">Package dependencies.</param>
        /// <param name="provider">Package provider info.</param>
        public PackageInfo(string name,
                           PackageVersion? version,
                           PackageSourceInfo? source,
                           string description,
                           IEnumerable<PackageDependency>? dependencies,
                           PackageProviderInfo provider)
            : this(name, version, description, provider)
        {
            Source = source;
            _dependencies = new List<PackageDependency>(dependencies);
        }

        /// <summary>
        /// Instantiates a <c>PackageInfo</c> object.
        /// </summary>
        /// <param name="name">Package name.</param>
        /// <param name="version">Package version.</param>
        /// <param name="source">Package source.</param>
        /// <param name="description">Package description.</param>
        /// <param name="dependencies">Package dependencies.</param>
        /// <param name="metadata">Additional package metadata.</param>
        /// <param name="provider">Package provider info.</param>
        public PackageInfo(string name,
                           PackageVersion? version,
                           PackageSourceInfo? source,
                           string description,
                           IEnumerable<PackageDependency>? dependencies,
                           IDictionary<string, object> metadata,
                           PackageProviderInfo provider)
            : this(name, version, source, description, dependencies, provider)
        {
            if (metadata is null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            Metadata = new ReadOnlyDictionary<string, object>(metadata);
        }

        /// <summary>
        /// Instantiates a <c>PackageInfo</c> object.
        /// </summary>
        /// <param name="name">Package name.</param>
        /// <param name="version">Package version.</param>
        /// <param name="source">Package source.</param>
        /// <param name="description">Package description.</param>
        /// <param name="dependencies">Package dependencies.</param>
        /// <param name="metadata">Additional package metadata.</param>
        /// <param name="provider">Package provider info.</param>
        public PackageInfo(string name,
                           PackageVersion? version,
                           PackageSourceInfo? source,
                           string description,
                           IEnumerable<PackageDependency>? dependencies,
                           Hashtable metadata,
                           PackageProviderInfo provider)
            : this(name, version, source, description, dependencies, provider)
        {
            if (metadata is null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            if (metadata.Count > 0)
            {
                var dictionary = new Dictionary<string, object>();

                foreach (var key in metadata.Keys)
                {
                    dictionary.Add(key.ToString(), metadata[key]);
                }

                Metadata = new ReadOnlyDictionary<string, object>(dictionary);
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
