// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using AnyPackage.Resources;

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
        public IReadOnlyDictionary<string, object?> Metadata { get; }

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
        /// <exception cref="ArgumentNullException">If name, location, or provider is null.</exception>
        /// <exception cref="ArgumentException">If name or location is empty or whitespace.</exception>
        public PackageSourceInfo(string name, string location, PackageProviderInfo provider)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (location is null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            if (provider is null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(Strings.NullOrWhiteSpace, nameof(name));
            }

            if (string.IsNullOrWhiteSpace(location))
            {
                throw new ArgumentException(Strings.NullOrWhiteSpace, nameof(location));
            }

            Name = name;
            Location = location;
            Provider = provider;
            Metadata = new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>());
        }

        /// <summary>
        /// Instantiates a <c>PackageSourceInfo</c> class.
        /// </summary>
        /// <param name="name">Source name.</param>
        /// <param name="location">Source location.</param>
        /// <param name="trusted">If source is trusted.</param>
        /// <param name="provider">Package provider.</param>
        /// <exception cref="ArgumentNullException">If name, location, or provider is null.</exception>
        /// <exception cref="ArgumentException">If name or location is empty or whitespace.</exception>
        public PackageSourceInfo(string name, string location, bool trusted, PackageProviderInfo provider)
            : this(name, location, provider)
        {
            Trusted = trusted;
        }

        /// <summary>
        /// Instantiates a <c>PackageSourceInfo</c> class.
        /// </summary>
        /// <param name="name">Source name.</param>
        /// <param name="location">Source location.</param>
        /// <param name="trusted">If source is trusted.</param>
        /// <param name="metadata">Additional metadata about source.</param>
        /// <param name="provider">Package provider.</param>
        /// <exception cref="ArgumentNullException">If name, location, provider, or metadata is null.</exception>
        /// <exception cref="ArgumentException">If name or location is empty or whitespace.</exception>
        public PackageSourceInfo(string name,
                                 string location,
                                 bool trusted,
                                 IDictionary<string, object?> metadata,
                                 PackageProviderInfo provider) : this(name, location, trusted, provider)
        {
            if (metadata is null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            Metadata = new ReadOnlyDictionary<string, object?>(metadata);
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
        /// <exception cref="ArgumentNullException">If name, location, provider, or metadata is null.</exception>
        /// <exception cref="ArgumentException">If name or location is empty or whitespace.</exception>
        public PackageSourceInfo(string name,
                                 string location,
                                 bool trusted,
                                 Hashtable metadata,
                                 PackageProviderInfo provider) : this(name, location, trusted, provider)
        {
            if (metadata is null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            if (metadata.Count > 0)
            {
                var dictionary = metadata.ToDictionary();
                Metadata = new ReadOnlyDictionary<string, object?>(dictionary);
            }
        }

        /// <summary>
        /// Instantiates a <c>PackageSourceInfo</c> class.
        /// </summary>
        /// <remarks>
        /// Metadata hashtable keys will be converted to strings.
        /// </remarks>
        /// <param name="psObject">A PSObject containing the key value pairs.</param>
        /// <exception cref="InvalidCastException">If properties are not of the correct type.</exception>
        /// <exception cref="NullReferenceException">If a required property is missing.</exception>
        public PackageSourceInfo(PSObject psObject)
        {
            Name = (string)psObject.Properties[nameof(Name)].Value;

            var provider = (PSObject)psObject.Properties[nameof(Provider)].Value;
            Provider = provider.BaseObject as PackageProviderInfo ?? throw new InvalidCastException();

            try
            {
                Location = (string)psObject.Properties[nameof(Location)].Value;
            }
            catch (NullReferenceException)
            {
                Location = Name;
            }

            try
            {
                Trusted = (bool)psObject.Properties[nameof(Trusted)].Value;
            }
            catch (NullReferenceException)
            {
                // Trusted property is optional.
            }

            IDictionary<string, object?> metadata;
            try
            {
                metadata = (IDictionary<string, object?>)psObject.Properties[nameof(Metadata)].Value;
            }
            catch (NullReferenceException)
            {
                metadata = new Dictionary<string, object?>();
            }
            catch (InvalidCastException)
            {
                var hashtable = psObject.Properties[nameof(Metadata)].Value as Hashtable
                    ?? throw new InvalidCastException();
                metadata = hashtable.ToDictionary();
            }

            Metadata = new ReadOnlyDictionary<string, object?>(metadata);
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
