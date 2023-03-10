// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Collections.Generic;

namespace AnyPackage.Provider
{
    /// <summary>
    /// The <c>PackageVersion</c> class.
    /// </summary>
    public sealed class PackageVersion : IComparable, IComparable<PackageVersion>, IEquatable<PackageVersion>
    {
        /// <summary>
        /// Gets the version.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Gets the dot separated first position.
        /// </summary>
        public int Major { get; }

        /// <summary>
        /// Gets the dot separated second position.
        /// </summary>
        public int Minor { get; }

        /// <summary>
        /// Gets the dot separated third position.
        /// </summary>
        public int Patch { get; }

        /// <summary>
        /// Gets the dot separated fourth position. 
        /// </summary>
        public int Revision { get; }

        /// <summary>
        /// Gets all the dot separated values.
        /// </summary>
        public IEnumerable<int> Decimal { get; }

        /// <summary>
        /// Gets if the version is a prerelease.
        /// </summary>
        public bool IsPrerelease { get; }

        /// <summary>
        /// Gets if the version contains a suffix.
        /// </summary>
        public bool HasSuffix { get; }

        /// <summary>
        /// Gets the suffix of a multi-part numeric with suffix version.
        /// </summary>
        public string Suffix { get; }

        /// <summary>
        /// Gets the dot separated values of the prerelease string.
        /// </summary>
        public IEnumerable<string> Prerelease { get; }

        /// <summary>
        /// Gets the dot separated values of the build metadata string.
        /// </summary>
        public IEnumerable<string> BuildMetadata { get; }

        /// <summary>
        /// Gets the version scheme.
        /// </summary>
        public PackageVersionScheme Scheme { get; }

        /// <summary>
        /// Constructs an instance of the <c>PackageVersion</c> class.
        /// </summary>
        /// <param name="version">The version.</param>
        public PackageVersion(string version)
        {
            // Suffix regex: ^(?<version>[\d\.]+)(?<suffix>[\w]*)$
            // Multi-part numeric: ^[\d\.]+$
   
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the string representation of a version to an equivalent <c>PackageVersion</c> object.
        /// </summary>
        /// <param name="version">A string that contains a version to convert.</param>
        /// <returns>
        /// An object that is equivalent to the version specified in the version parameter.
        /// </returns>
        public static PackageVersion Parse(string version) 
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tries to convert the string representation of a version to an equivalent <c>PackageVersion</c> object,
        /// and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="version">A string that contains a version to convert.</param>
        /// <param name="result">
        /// When this methods returns, contains the <c>PackageVersion</c> equivalent of the string, if the conversion succeeded.
        /// If <c>version</c> is <c>null</c>, Empty, or if the conversion fails, <c>result</c> is <c>null</c> when the method returns.
        /// </param>
        /// <returns>
        /// <c>true</c> if the <c>version</c> parameter was converted successfully; otherwise, <c>false</c>.
        /// </returns>
        public static bool TryParse(string version, out PackageVersion result) 
        {
            throw new NotImplementedException();
        }
        
        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(PackageVersion other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(PackageVersion other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj) => Version.Equals(obj);

        public override int GetHashCode() => Version.GetHashCode();

        public override string ToString() => Version;

        // TODO: Override operators
    }
}
