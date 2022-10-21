// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Collections;

namespace UniversalPackageManager.Provider
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
        /// TODO: Change to ImmutableDictionary<string, object>
        public Hashtable Metadata { get; }

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
        internal PackageSourceInfo(string name, string location, PackageProviderInfo provider, bool trusted, Hashtable? metadata)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or whitespace.");
            }
            
            Name = name;
            Location = location;
            Provider = provider;
            Trusted = trusted;
            Metadata = metadata ?? new Hashtable();
        }

        /// <summary>
        /// Returns package source name.
        /// </summary>
        public override string ToString()
        {
            return Name;
        }
    }
}
