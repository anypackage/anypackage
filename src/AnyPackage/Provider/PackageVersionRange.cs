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
        /// Constructs a version range using a PowerShell modified NuGet package version range syntax.
        /// </summary>
        /// <remarks>
        /// <para>
        /// To maintain type conversion compatibility with the PowerShell cmdlet parameters
        /// the default behavior is to change min version syntax into required.
        /// For example, <c>1.0</c> would become <c>[1.0]</c> since PowerShell users expect the version
        /// to be the exact version supplied instead of NuGet syntax of min version inclusive.
        /// To pass a min version inclusive use <c>[1.0,]</c> syntax.
        /// </para>
        /// This behavior is only used when constructing the version range.
        /// The standard NuGet syntax is used elsewhere.
        /// If you need to use NuGet syntax use the constructor with useNuGetSyntax parameter.
        /// </remarks>
        /// <param name="versionRange">NuGet package version formatted string.</param>
        /// <exception cref="ArgumentNullException">Version range is null.</exception>
        /// <exception cref="ArgumentException">Version range is not in correct format.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Min version is higher than max version.</exception>
        public PackageVersionRange(string versionRange) : this(versionRange, false) { }

        /// <summary>
        /// Constructs a version range using a NuGet package version range syntax.
        /// </summary>
        /// <remarks>
        /// <para>
        /// To maintain type conversion compatibility with the PowerShell cmdlet parameters
        /// the default behavior is to change min version syntax into required.
        /// For example, 1.0 would become [1.0] since PowerShell users expect the version
        /// to be the exact version supplied instead of NuGet syntax of min version inclusive.
        /// To pass a min version inclusive use <c>[1.0,]</c> syntax.
        /// </para>
        /// This behavior is only used when constructing the version range.
        /// The standard NuGet syntax is used elsewhere.
        /// If you need to use NuGet syntax use the useNuGetSyntax parameter.
        /// </remarks>
        /// <param name="versionRange">NuGet package version formatted string.</param>
        /// <param name="useNuGetSyntax">
        /// If <c>true</c> use NuGet syntax otherwise <c>false</c> uses PowerShell modified syntax.
        /// </param>
        /// <exception cref="ArgumentNullException">Version range is null.</exception>
        /// <exception cref="ArgumentException">Version range is not in correct format.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Min version is higher than max version.</exception>
        public PackageVersionRange(string versionRange, bool useNuGetSyntax)
        {
            if (versionRange is null)
            {
                throw new ArgumentNullException(versionRange);
            }
            
            if (string.IsNullOrWhiteSpace(versionRange))
            {
                throw new ArgumentException(nameof(versionRange), "Cannot be null or whitespace.");
            }

            if (versionRange == "*") { return; }

            var first = versionRange[0];
            var last = versionRange[versionRange.Length - 1];

            if (first != '[' && first != '(' && last != ']' && last != ')')
            {
                MinVersion = new PackageVersion(versionRange);
                IsMinInclusive = true;
                
                if (!useNuGetSyntax)
                {
                    MaxVersion = MinVersion;
                    IsMinInclusive = true;
                    IsMaxInclusive = true;
                }

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

            if (MinVersion is not null
                && MaxVersion is not null
                && MinVersion > MaxVersion)
            {
                throw new ArgumentOutOfRangeException(nameof(versionRange), "Min version is higher than max version.");
            }
        }

        /// <summary>
        /// Constructs a version range using min and max version.
        /// </summary>
        /// <param name="minVersion">Minimum version.</param>
        /// <param name="maxVersion">Maximum version.</param>
        /// <param name="isMinInclusive">If min version is inclusive.</param>
        /// <param name="isMaxInclusive">If max version is inclusive.</param>
        /// <exception cref="ArgumentOutOfRangeException">Min version is higher than max version.</exception>
        public PackageVersionRange(PackageVersion? minVersion = null,
                                   PackageVersion? maxVersion = null,
                                   bool isMinInclusive = true,
                                   bool isMaxInclusive = false)
        {
            if (minVersion is not null
                && maxVersion is not null
                && minVersion > maxVersion)
            {
                throw new ArgumentOutOfRangeException(nameof(minVersion), "Min version is higher than max version.");
            }

            if (minVersion is not null)
            {
                MinVersion = minVersion;
                IsMinInclusive = isMinInclusive;
            }
            else
            {
                IsMaxInclusive = false;
            }

            if (maxVersion is not null)
            {
                MaxVersion = maxVersion;
                IsMaxInclusive = isMaxInclusive;
            }
            else
            {
                IsMaxInclusive = false;
            }
        }

        /// <summary>
        /// Converts the string representation of a version range to an equivalent <c>PackageVersionRange</c> object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// To maintain type conversion compatibility with the PowerShell cmdlet parameters
        /// the default behavior is to change min version syntax into required.
        /// For example, <c>1.0</c> would become <c>[1.0]</c> since PowerShell users expect the version
        /// to be the exact version supplied instead of NuGet syntax of min version inclusive.
        /// To pass a min version inclusive use <c>[1.0,]</c> syntax.
        /// </para>
        /// This behavior is only used when constructing the version range.
        /// The standard NuGet syntax is used elsewhere.
        /// If you need to use NuGet syntax use the Parse method with useNuGetSyntax parameter.
        /// </remarks>
        /// <param name="versionRange">A string that contains a version range to convert.</param>
        /// <returns>
        /// An object that is equivalent to the version range specified in the versionRange parameter.
        /// </returns>
        /// <exception cref="ArgumentNullException">Version range is null.</exception>
        /// <exception cref="ArgumentException">Version range is not in correct format.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Min version is higher than max version.</exception>
        public static PackageVersionRange Parse(string versionRange)
            => new PackageVersionRange(versionRange);

        /// <summary>
        /// Converts the string representation of a version range to an equivalent <c>PackageVersionRange</c> object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// To maintain type conversion compatibility with the PowerShell cmdlet parameters
        /// the default behavior is to change min version syntax into required.
        /// For example, <c>1.0</c> would become <c>[1.0]</c> since PowerShell users expect the version
        /// to be the exact version supplied instead of NuGet syntax of min version inclusive.
        /// To pass a min version inclusive use <c>[1.0,]</c> syntax.
        /// </para>
        /// This behavior is only used when constructing the version range.
        /// The standard NuGet syntax is used elsewhere.
        /// If you need to use NuGet syntax use the useNuGetSyntax parameter.
        /// </remarks>
        /// <param name="versionRange">A string that contains a version range to convert.</param>
        /// <param name="useNuGetSyntax">
        /// If <c>true</c> use NuGet syntax otherwise <c>false</c> uses PowerShell modified syntax.
        /// </param>
        /// <returns>
        /// An object that is equivalent to the version range specified in the versionRange parameter.
        /// </returns>
        /// <exception cref="ArgumentNullException">Version range is null.</exception>
        /// <exception cref="ArgumentException">Version range is not in correct format.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Min version is higher than max version.</exception>
        public static PackageVersionRange Parse(string versionRange, bool useNuGetSyntax)
            => new PackageVersionRange(versionRange, useNuGetSyntax);

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
        /// Tries to convert the string representation of a
        /// version to an equivalent <c>PackageVersionRange</c> object,
        /// and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <remarks>
        /// <para>
        /// To maintain type conversion compatibility with the PowerShell cmdlet parameters
        /// the default behavior is to change min version syntax into required.
        /// For example, <c>1.0</c> would become <c>[1.0]</c> since PowerShell users expect the version
        /// to be the exact version supplied instead of NuGet syntax of min version inclusive.
        /// To pass a min version inclusive use <c>[1.0,]</c> syntax.
        /// </para>
        /// This behavior is only used when constructing the version range.
        /// The standard NuGet syntax is used elsewhere.
        /// If you need to use NuGet syntax use the useNuGetSyntax parameter.
        /// </remarks>
        /// <param name="versionRange">A string that contains a version range to convert.</param>
        /// <param name="useNuGetSyntax">
        /// If <c>true</c> use NuGet syntax otherwise <c>false</c> uses PowerShell modified syntax.
        /// </param>
        /// <param name="result">
        /// When this methods returns, contains the <c>PackageVersionRange</c> equivalent of the string, if the conversion succeeded.
        /// If <c>version</c> is <c>null</c>, Empty, or if the conversion fails, <c>result</c> is <c>null</c> when the method returns.
        /// </param>
        /// <returns>
        /// <c>true</c> if the <c>version</c> parameter was converted successfully; otherwise, <c>false</c>.
        /// </returns>
        public static bool TryParse(string versionRange, bool useNuGetSyntax, out PackageVersionRange? result)
        {
            try
            {
                result = new PackageVersionRange(versionRange, useNuGetSyntax);
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
        public override string ToString() => ToString(false);

        /// <summary>
        /// Provides a <c>ToString</c> implementation.
        /// </summary>
        /// <param name="shortNotation">Min version inclusive will return just the version.</param>
        /// <returns>A NuGet package version reference string with short notation.</returns>
        public string ToString(bool shortNotation)
        {
            if (MinVersion is null && MaxVersion is null)
            {
                return "*";
            }
            else if (MinVersion is not null && MaxVersion is null)
            {
                if (IsMinInclusive && shortNotation)
                {
                    return MinVersion.ToString();
                }
                else if (IsMinInclusive)
                {
                    return string.Format("[{0},)", MinVersion);
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
                var lhs = IsMinInclusive ? '[' : '(';
                var rhs = IsMaxInclusive ? ']' : ')';

                return string.Format("{0}{1},{2}{3}", lhs, MinVersion, MaxVersion, rhs);
            }
        }
    }
}
