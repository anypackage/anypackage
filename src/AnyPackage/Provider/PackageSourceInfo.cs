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
    /// The <c>PackageSourceInfo</c> class.
    /// Contains information regarding a package source.
    /// </summary>
    public sealed class PackageSourceInfo
    {
        /// <summary>
        /// Gets the source name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the source location.
        /// </summary>
        public string Location { get; }

        /// <summary>
        /// Gets the package provider information.
        /// </summary>
        public PackageProviderInfo Provider { get; }

        /// <summary>
        /// Gets source metadata.
        /// </summary>
        public IReadOnlyDictionary<string, object> Metadata { get; }

        /// <summary>
        /// Gets if the source is trusted.
        /// </summary>
        public bool Trusted { get; }

        /// <summary>
        /// Instantiates a <c>PackageSourceInfo</c> class.
        /// </summary>
        /// <param name="name">Source name.</param>
        /// <param name="location">Source location.</param>
        /// <param name="provider">Package provider.</param>
        /// <param name="trusted">If source is trusted.</param>
        /// <param name="metadata">Additional metadata about source.</param>
        internal PackageSourceInfo(string name,
                                   string location,
                                   PackageProviderInfo provider,
                                   bool trusted,
                                   IDictionary<string, object>? metadata) : this(name, location, provider, trusted)
        {
            if (metadata is not null)
            {
                Metadata = new ReadOnlyDictionary<string, object>(metadata);
            }
        }

        /// <summary>
        /// Instantiates a <c>PackageSourceInfo</c> class.
        /// </summary>
        /// <remarks>
        /// Metadata hashtable keys will be converted to strings.
        /// </remarks>
        /// <param name="name">Source name.</param>
        /// <param name="location">Source location.</param>
        /// <param name="provider">Package provider.</param>
        /// <param name="trusted">If source is trusted.</param>
        /// <param name="metadata">Additional metadata about source.</param>
        internal PackageSourceInfo(string name,
                                   string location,
                                   PackageProviderInfo provider,
                                   bool trusted,
                                   Hashtable? metadata) : this(name, location, provider, trusted)
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

        private PackageSourceInfo(string name, string location, PackageProviderInfo provider, bool trusted)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Cannot be null or whitespace.", nameof(name));
            }

            if (string.IsNullOrWhiteSpace(location))
            {
                throw new ArgumentException("Cannot be null or whitespace.", nameof(location));
            }

            Name = name;
            Location = location;
            Provider = provider;
            Trusted = trusted;
            Metadata = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>());
        }

        /// <summary>
        /// Returns a string of the source name.
        /// </summary>
        /// <returns>
        /// The source name.
        /// </returns>
        public override string ToString() => Name;
    }
}
