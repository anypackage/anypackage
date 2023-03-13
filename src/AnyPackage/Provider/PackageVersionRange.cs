// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace AnyPackage.Provider
{
    /// <summary>
    /// The <c>PackageVersionRange</c>.
    /// </summary>
    public sealed class PackageVersionRange
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
        public bool IsMinInclusive { get; }

        /// <summary>
        /// Gets if the maximum version is inclusive.
        /// </summary>
        public bool IsMaxInclusive { get; }

        /// <summary>
        /// Constructs a version range using a NuGet package version range syntax.
        /// </summary>
        /// <param name="versionRange">NuGet package version formatted string.</param>
        public PackageVersionRange(string versionRange)
        {
            if (string.IsNullOrWhiteSpace(versionRange))
            {
                throw new ArgumentNullException(nameof(versionRange), "Cannot be null or whitespace.");
            }

            var first = versionRange[0];
            var last = versionRange[versionRange.Length - 1];

            if (first != '[' && first != '(' && last != ']' && last != ')')
            {
                MinVersion = new PackageVersion(versionRange);
                IsMinInclusive = true;
                return;
            }

            switch (first)
            {
                case '[':
                    IsMinInclusive = true;
                    break;
                case '(':
                    IsMinInclusive = false;
                    break;
                default:
                    throw new ArgumentException(nameof(versionRange), "Invalid format.");
            }

            switch (last)
            {
                case ']':
                    IsMaxInclusive = true;
                    break;
                case ')':
                    IsMaxInclusive = false;
                    break;
                default:
                    throw new ArgumentException(nameof(versionRange), "Invalid format.");
            }

            // Strip off opening and closing characters
            var nuGetVersion = versionRange.Substring(1, versionRange.Length - 2);

            // Split on comma, to get both version parts
            string[] parts = nuGetVersion.Split(',');

            if ((parts.Length > 2 ||
                 parts.All(string.IsNullOrEmpty)) ||
                 (parts.Length == 1 && first == '(' && last == ')'))
            {
                throw new ArgumentException(nameof(nuGetVersion), "Invalid format.");
            }

            var minimumVersion = parts[0];
            var maximumVersion = parts.Length == 2 ? parts[1] : parts[0];

            if (!string.IsNullOrWhiteSpace(minimumVersion))
            {
                MinVersion = new PackageVersion(minimumVersion);
            }

            if (!string.IsNullOrWhiteSpace(maximumVersion))
            {
                MaxVersion = new PackageVersion(maximumVersion);
            }
        }

        /// <summary>
        /// Constructs a version range using min and max version.
        /// </summary>
        /// <param name="minVersion">Minimum version.</param>
        /// <param name="maxVersion">Maximum version.</param>
        /// <param name="isMinInclusive">If min version is inclusive.</param>
        /// <param name="isMaxInclusive">If max version is inclusive.</param>
        public PackageVersionRange(PackageVersion? minVersion = null,
                                   PackageVersion? maxVersion = null,
                                   bool isMinInclusive = true,
                                   bool isMaxInclusive = true)
        {
            MinVersion = minVersion;
            MaxVersion = maxVersion;
            IsMinInclusive = isMinInclusive;
            IsMaxInclusive = isMaxInclusive;
        }

        /// <summary>
        /// Converts the string representation of a version range to an equivalent <c>PackageVersionRange</c> object.
        /// </summary>
        /// <param name="versionRange">A string that contains a version range to convert.</param>
        /// <returns>
        /// An object that is equivalent to the version range specified in the versionRange parameter.
        /// </returns>
        public static PackageVersionRange Parse(string versionRange) => new PackageVersionRange(versionRange);

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
        public static bool TryParse(string versionRange, out PackageVersionRange? result)
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

        /// <summary>
        /// Checks the supplied version satisfies the version range.
        /// </summary>
        /// <param name="version">The package version to check.</param>
        /// <returns>Returns <c>true</c> if the version range is satisfied by the supplied version.</returns>
        public bool Satisfies(PackageVersion version)
        {
            if (version is null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            var satisfies = true;

            if (MinVersion is not null)
            {
                if (IsMinInclusive)
                {
                    satisfies &= MinVersion.CompareTo(version) <= 0;
                }
                else
                {
                    satisfies &= MinVersion.CompareTo(version) < 0;
                }
            }

            if (MaxVersion is not null)
            {
                if (IsMaxInclusive)
                {
                    satisfies &= MaxVersion.CompareTo(version) >= 0;
                }
                else
                {
                    satisfies &= MaxVersion.CompareTo(version) > 0;
                }
            }

            return satisfies;
        }

        /// <summary>
        /// Checks the supplied version satisfies the version range.
        /// </summary>
        /// <param name="version">The package version to check.</param>
        /// <param name="comparer">The custom comparer to use for version comparisons.</param>
        /// <returns>Returns <c>true</c> if the version range is satisfied by the supplied version.</returns>
        public bool Satisfies(PackageVersion version, IComparer<PackageVersion> comparer)
        {
            if (version is null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            var satisfies = true;

            if (MinVersion is not null)
            {
                if (IsMinInclusive)
                {
                    satisfies &= comparer.Compare(MinVersion, version) <= 0;
                }
                else
                {
                    satisfies &= comparer.Compare(MinVersion, version) < 0;
                }
            }

            if (MaxVersion is not null)
            {
                if (IsMaxInclusive)
                {
                    satisfies &= comparer.Compare(MaxVersion, version) >= 0;
                }
                else
                {
                    satisfies &= comparer.Compare(MaxVersion, version) > 0;
                }
            }

            return satisfies;
        }

        /// <summary>
        /// Provides a <c>ToString</c> implementation.
        /// </summary>
        /// <remarks>
        /// For more information refer to: https://docs.microsoft.com/en-us/nuget/concepts/package-versioning
        /// </remarks>
        /// <returns>A NuGet package version reference string.</returns>
        public override string ToString()
        {
            if (MinVersion is not null && MaxVersion is null)
            {
                if (IsMinInclusive)
                {
                    return MinVersion.ToString();
                }
                else
                {
                    return string.Format("({0},)", MinVersion);
                }
            }
            else if (MinVersion is null && MaxVersion is not null)
            {
                if (IsMaxInclusive)
                {
                    return string.Format("(,{0}]", MaxVersion);
                }
                else
                {
                    return string.Format("(,{0})", MaxVersion);
                }
            }
            else if (MinVersion is not null
                     && MaxVersion is not null
                     && MinVersion == MaxVersion)
            {
                return string.Format("[{0}]", MinVersion);
            }
            else
            {
                // TODO: May need to check if (1.0,2.0] is valid
                var lhs = IsMinInclusive ? '[' : '(';
                var rhs = IsMaxInclusive ? ']' : ')';

                return string.Format("{0}{1},{2}{3}", lhs, MinVersion, MaxVersion, rhs);
            }
        }
    }
}
