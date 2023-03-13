// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;

namespace AnyPackage.Provider
{
    /// <summary>
    /// The <c>PackageVersionRange</c>.
    /// </summary>
    public class PackageVersionRange
    {
        /// <summary>
        /// Gets the minimum package version.
        /// </summary>
        public PackageVersion? MinVersion { get; }

        /// <summary>
        /// Gets the maximum package version.
        /// </summary>
        public PackageVersion? MaxVersion { get; }

        /// <summary>
        /// Gets if the minimum version is inclusive.
        /// </summary>
        public bool IsMinVersionInclusive { get; }

        /// <summary>
        /// Gets if the maximum version is inclusive.
        /// </summary>
        public bool IsMaxVersionInclusive { get; }

        /// <summary>
        /// Constructs a version range using a NuGet package version range syntax.
        /// </summary>
        /// <param name="versionRange">NuGet package version formatted string.</param>
        public PackageVersionRange(string versionRange)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Constructs a version range using min and max version.
        /// </summary>
        /// <param name="minVersion">Minimum version.</param>
        /// <param name="maxVersion">Maximum version.</param>
        /// <param name="isMinVersionInclusive">If min version is inclusive.</param>
        /// <param name="isMaxVersionInclusive">If max version is inclusive.</param>
        public PackageVersionRange(PackageVersion? minVersion = null,
                                   PackageVersion? maxVersion = null,
                                   bool isMinVersionInclusive = true,
                                   bool isMaxVersionInclusive = true)
        {
            MinVersion = minVersion;
            MaxVersion = maxVersion;
            IsMinVersionInclusive = isMaxVersionInclusive;
            IsMaxVersionInclusive = IsMaxVersionInclusive;
        }

        /// <summary>
        /// Converts the string representation of a version range to an equivalent <c>PackageVersionRange</c> object.
        /// </summary>
        /// <param name="versionRange">A string that contains a version range to convert.</param>
        /// <returns>
        /// An object that is equivalent to the version range specified in the versionRange parameter.
        /// </returns>
        public PackageVersionRange Parse(string versionRange) => new PackageVersionRange(versionRange);

        /// <summary>
        /// Tries to convert the string representation of a
        /// version to an equivalent <c>PackageVersionRange</c> object,
        /// and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="versionRange">A string that contains a version range to convert.</param>
        /// <param name="result">
        /// When this methods returns, contains the <c>PackageVersionRange</c> equivalent of the string, if the conversion succeeded.
        /// If <c>version</c> is <c>null</c>, Empty, or if the conversion fails, <c>result</c> is <c>null</c> when the method returns.
        /// </param>
        /// <returns>
        /// <c>true</c> if the <c>version</c> parameter was converted successfully; otherwise, <c>false</c>.
        /// </returns>
        public bool TryParse(string versionRange, out PackageVersionRange? result)
        {
            try
            {
                result = new PackageVersionRange(versionRange);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
}
