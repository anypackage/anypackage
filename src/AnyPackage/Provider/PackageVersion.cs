// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AnyPackage.Provider
{
    /// <summary>
    /// The <c>PackageVersion</c> class.
    /// </summary>
    public sealed class PackageVersion : IComparable, IComparable<PackageVersion>, IEquatable<PackageVersion>
    {
        private static readonly Regex s_semVer = new(@"^(?<major>0|[1-9]\d*)\.(?<minor>0|[1-9]\d*)\.(?<patch>0|[1-9]\d*)(?:-(?<prerelease>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?<buildMetadata>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$");
        private static readonly Regex s_multiPartNumericSuffix = new(@"^(?<version>((\d+\.)+\d+))(?<suffix>[a-zA-Z]\w*)$");
        private static readonly Regex s_multiPartNumeric = new(@"^(\d+\.)+\d+$");
        private static readonly Regex s_integer = new(@"^\d+$");

        /// <summary>
        /// Gets the version.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Gets the dot separated first position.
        /// </summary>
        public ulong? Major => _parts.Count > 0 ? _parts[0] : null;

        /// <summary>
        /// Gets the dot separated second position.
        /// </summary>
        public ulong? Minor => _parts.Count > 1 ? _parts[1] : null;

        /// <summary>
        /// Gets the dot separated third position.
        /// </summary>
        public ulong? Patch => _parts.Count > 2 ? _parts[2] : null;

        /// <summary>
        /// Gets the dot separated fourth position. 
        /// </summary>
        public ulong? Revision => _parts.Count > 3 ? _parts[3] : null;

        /// <summary>
        /// Gets all the dot separated values.
        /// </summary>
        public IEnumerable<ulong> Parts => _parts;

        /// <summary>
        /// Gets if the version is a prerelease.
        /// </summary>
        public bool IsPrerelease => _prerelease.Count > 0;

        /// <summary>
        /// Gets the suffix of a multi-part numeric with suffix version.
        /// </summary>
        public string? Suffix { get; }

        /// <summary>
        /// Gets if there
        /// </summary>
        public bool HasMetadata => _metadata.Count > 0;

        /// <summary>
        /// Gets the dot separated values of the prerelease string.
        /// </summary>
        public IEnumerable<string> Prerelease => _prerelease;

        /// <summary>
        /// Gets the dot separated values of the build metadata string.
        /// </summary>
        public IEnumerable<string> Metadata => _metadata;

        /// <summary>
        /// Gets the version scheme.
        /// </summary>
        public PackageVersionScheme Scheme { get; }

        private readonly List<ulong> _parts = [];
        private readonly List<string> _prerelease = [];
        private readonly List<string> _metadata = [];

        /// <summary>
        /// Constructs an instance of the <c>PackageVersion</c> class.
        /// </summary>
        /// <param name="version">The version.</param>
        public PackageVersion(string version)
        {
            if (version is null)
            {
                throw new ArgumentNullException(version);
            }

            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentException("Value cannot be whitespace.", version);
            }

            Match match;

            if (s_semVer.IsMatch(version))
            {
                match = s_semVer.Match(version);
                _parts.Add(ulong.Parse(match.Groups[1].Value));
                _parts.Add(ulong.Parse(match.Groups[2].Value));
                _parts.Add(ulong.Parse(match.Groups[3].Value));

                if (match.Groups[4].Success)
                {
                    var prerelease = match.Groups[4].Value;

                    if (prerelease.Contains("."))
                    {
                        foreach (var section in prerelease.Split('.'))
                        {
                            _prerelease.Add(section);
                        }
                    }
                    else
                    {
                        _prerelease.Add(prerelease);
                    }
                }

                if (match.Groups[5].Success)
                {
                    var buildMetadata = match.Groups[5].Value;

                    if (buildMetadata.Contains("."))
                    {
                        foreach (var section in buildMetadata.Split('.'))
                        {
                            _metadata.Add(section);
                        }
                    }
                    else
                    {
                        _metadata.Add(buildMetadata);
                    }
                }

                Scheme = PackageVersionScheme.SemanticVersion;
            }
            else if (s_multiPartNumericSuffix.IsMatch(version))
            {
                match = s_multiPartNumericSuffix.Match(version);
                SetParts(match.Groups[match.Groups.Count - 2].Value);
                Suffix = match.Groups[match.Groups.Count - 1].Value;
                Scheme = PackageVersionScheme.MultiPartNumericSuffix;
            }
            else if (s_multiPartNumeric.IsMatch(version))
            {
                match = s_multiPartNumeric.Match(version);
                SetParts(match.Captures[0].Value);
                Scheme = PackageVersionScheme.MultiPartNumeric;
            }
            else if (s_integer.IsMatch(version))
            {
                match = s_integer.Match(version);
                _parts.Add(ulong.Parse(match.Captures[0].Value));
                Scheme = PackageVersionScheme.Integer;
            }
            else
            {
                Scheme = PackageVersionScheme.AlphaNumeric;
            }

            Version = version;
        }

        /// <summary>
        /// Constructs an instance of the <c>PackageVersion</c> class.
        /// </summary>
        /// <param name="version">The version.</param>
        public PackageVersion(Version version)
        {
            Version = version.ToString();
            Scheme = PackageVersionScheme.MultiPartNumeric;
            _parts.Add((ulong)version.Major);
            _parts.Add((ulong)version.Minor);

            if (version.Build != -1) { _parts.Add((ulong)version.Build); }
            if (version.Revision != -1) { _parts.Add((ulong)version.Revision); }
        }

        /// <summary>
        /// Implements the == (equality) operator.
        /// </summary>
        /// <param name="left">LHS package version.</param>
        /// <param name="right">RHS package version.</param>
        /// <returns>
        /// Returns true if the left hand side version is equal to the right hand side.
        /// </returns>
        public static bool operator ==(PackageVersion left, PackageVersion right) => left.Equals(right);

        /// <summary>
        /// Implements the != (inequality) operator.
        /// </summary>
        /// <param name="left">LHS package version.</param>
        /// <param name="right">RHS package version.</param>
        /// <returns>
        /// Returns true if the left hand side version is not equal to the right hand side.
        /// </returns>
        public static bool operator !=(PackageVersion left, PackageVersion right) => !left.Equals(right);

        /// <summary>
        /// Implements the &lt; (less-than) operator.
        /// </summary>
        /// <param name="left">LHS package version.</param>
        /// <param name="right">RHS package version.</param>
        /// <returns>
        /// Returns true if the left hand side is lower version than right hand side.
        /// </returns>
        public static bool operator <(PackageVersion left, PackageVersion right) => left.CompareTo(right) < 0;

        /// <summary>
        /// Implements the &gt; (greater-than) operator.
        /// </summary>
        /// <param name="left">LHS package version.</param>
        /// <param name="right">RHS package version.</param>
        /// <returns>
        /// Returns true if the left hand side version is lower than right hand side.
        /// </returns>
        public static bool operator >(PackageVersion left, PackageVersion right) => left.CompareTo(right) > 0;

        /// <summary>
        /// Implements the &lt;= (less-than or equal to) operator. 
        /// </summary>
        /// <param name="left">LHS package version.</param>
        /// <param name="right">RHS package version.</param>
        /// <returns>
        /// Returns true if the left hand side version is lower or equal to right hand side.
        /// </returns>
        public static bool operator <=(PackageVersion left, PackageVersion right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// Implements the &gt;= (greater-than or equals to) operator.
        /// </summary>
        /// <param name="left">LHS package version.</param>
        /// <param name="right">RHS package version.</param>
        /// <returns>
        /// Returns true if the left hand side version is higher or equal to right hand side.
        /// </returns>
        public static bool operator >=(PackageVersion left, PackageVersion right) => left.CompareTo(right) >= 0;

        /// <summary>
        /// Implicit cast operator for casing <c>string</c> to <c>PackageVersion</c>.
        /// </summary>
        /// <param name="version">String representation of version.</param>
        public static implicit operator PackageVersion(string version) => new(version);

        /// <summary>
        /// Implicit cast operator for casing <c>Version</c> to <c>PackageVersion</c>.
        /// </summary>
        /// <param name="version">The Version object to convert to PackageVersion.</param>
        /// <returns>The corresponding PackageVersion object.</returns>
        public static implicit operator PackageVersion(Version version) => new(version);

        /// <summary>
        /// Explicit cast operator for casting <c>PackageVersion</c> to <c>Version</c>.
        /// </summary>
        /// <param name="version">The PackageVersion object to convert to Version.</param>
        /// <returns>The corresponding Version object.</returns>
        public static explicit operator Version(PackageVersion version) => version.ToVersion();

        /// <summary>
        /// Converts the string representation of a version to an equivalent <c>PackageVersion</c> object.
        /// </summary>
        /// <param name="version">A string that contains a version to convert.</param>
        /// <returns>
        /// An object that is equivalent to the version specified in the version parameter.
        /// </returns>
        public static PackageVersion Parse(string version) => new(version);

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
        public static bool TryParse(string version, out PackageVersion? result)
        {
            try
            {
                result = new PackageVersion(version);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// A Version representation of <c>PackageVersion</c>.
        /// </summary>
        /// <returns>A Version representation of <c>PackageVersion</c>.</returns>
        public Version ToVersion()
        {
            if (Scheme == PackageVersionScheme.AlphaNumeric)
            {
                throw new InvalidOperationException("Cannot convert alpha-numeric versions to Version type.");
            }

            if (_parts.Count > 4)
            {
                throw new InvalidOperationException("Version contains more than four parts.");
            }

            if (_parts.Count < 2)
            {
                throw new InvalidOperationException("Version contains less than two parts.");
            }

            if (IsPrerelease)
            {
                throw new InvalidOperationException("Version contains prerelease.");
            }

            if (HasMetadata)
            {
                throw new InvalidOperationException("Version contains build metadata.");
            }

            return System.Version.Parse(Version);
        }

        /// <summary>
        /// Compares this object to the other for version comparison.
        /// </summary>
        /// <remarks>
        /// Refer to <c>CompareTo(PackageVersion other)</c> for
        /// version comparison rules.
        /// </remarks>
        /// <param name="obj">The version to compare against</param>
        /// <returns>
        /// Returns -1 when this object is a lower version than the other.
        /// Returns 0 when this object and other object are equal.
        /// Returns 1 for this object is a higher version than the other.
        /// </returns>
        public int CompareTo(object? obj)
        {
            var version = obj as PackageVersion;
            return CompareTo(version);
        }

        /// <summary>
        /// Compares this object to the other for version comparison.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The sorting is by first comparing if both versions are using
        /// the alpha-numeric version scheme. If so then it compares using
        /// string comparison. If one of the two versions is alpha-numeric
        /// it be considered higher compared to the numeric version.
        /// </para>
        /// <para>
        /// Numeric sorting takes place after it is determined neither is
        /// alpha-numeric. It will compare each version part numerically until
        /// one part is different. The version sorting is able to compare different
        /// version lengths (1.0 vs 1.0.0 or 1.0.0.1). If no difference is found
        /// it will then compare suffix and prerelease.
        /// </para>
        /// <para>
        /// Versions with a suffix are considered higher than their non-suffix
        /// counterparts (1.0 is lower than 1.0a). If both versions contain
        /// a suffix they will compared using the string comparison.
        /// </para>
        /// <para>
        /// Versions with a prerelease are considered lower than their non-prerelease
        /// counterparts (1.0 is higher than 1.0-alpha). If both versions contain a
        /// prerelease they will compared using the rules defined in semver 2.0.
        /// </para>
        /// </remarks>
        /// <param name="other">The version to compare against.</param>
        /// <returns>
        /// Returns -1 when this object is a lower version than the other.
        /// Returns 0 when this object and other object are equal.
        /// Returns 1 for this object is a higher version than the other.
        /// </returns>
        public int CompareTo(PackageVersion? other)
        {
            if (other is null) { return 1; }

            if (CompareToScheme(other, out var result))
            {
                return result;
            }

            if (CompareToParts(other, out result))
            {
                return result;
            }

            if (CompareToSuffix(other, out result))
            {
                return result;
            }

            if (CompareToPrerelease(other, out result))
            {
                return result;
            }

            return 0;
        }

        /// <summary>
        /// Provides an Equals implementation.
        /// </summary>
        /// <param name="other">Input object.</param>
        /// <returns>Returns true if the objects are equal.</returns>
        public bool Equals(PackageVersion? other)
        {
            return Version.Equals(other?.Version);
        }

        /// <summary>
        /// Provides an Equals implementation.
        /// </summary>
        /// <param name="obj">Input object.</param>
        /// <returns>Returns true if the objects are equal.</returns>
        public override bool Equals(object? obj)
        {
            var version = obj as PackageVersion;
            return Equals(version);
        }

        /// <summary>
        /// Provides a GetHashCode implementation.
        /// </summary>
        /// <returns>Returns the hash code for Version property.</returns>
        public override int GetHashCode() => Version.GetHashCode();

        /// <summary>
        /// Provides a ToString implementation.
        /// </summary>
        /// <returns>Returns the Version property.</returns>
        public override string ToString() => Version;

        private bool CompareToScheme(PackageVersion other, out int result)
        {
            if (Scheme == PackageVersionScheme.AlphaNumeric && other.Scheme == PackageVersionScheme.AlphaNumeric)
            {
                result = Version.CompareTo(other.Version);
                return true;
            }
            else if (Scheme == PackageVersionScheme.AlphaNumeric)
            {
                result = 1;
                return true;
            }
            else if (other.Scheme == PackageVersionScheme.AlphaNumeric)
            {
                result = -1;
                return true;
            }

            result = int.MinValue;
            return false;
        }

        private bool CompareToParts(PackageVersion other, out int result)
        {
            var count = _parts.Count > other._parts.Count ? _parts.Count : other._parts.Count;
            ulong a, b;

            for (var i = 0; i < count; i++)
            {
                a = i < _parts.Count ? _parts[i] : 0;
                b = i < other._parts.Count ? other._parts[i] : 0;

                if (a != b)
                {
                    result = a.CompareTo(b);
                    return true;
                }
            }

            result = int.MinValue;
            return false;
        }

        private bool CompareToSuffix(PackageVersion other, out int result)
        {
            if (Suffix is not null && other.Suffix is not null)
            {
                result = Suffix.CompareTo(other.Suffix);
                return true;
            }
            else if (Suffix is not null)
            {
                result = 1;
                return true;
            }
            else if (other.Suffix is not null)
            {
                result = -1;
                return true;
            }

            result = int.MinValue;
            return false;
        }

        private bool CompareToPrerelease(PackageVersion other, out int result)
        {
            if (IsPrerelease && other.IsPrerelease)
            {
                var count = _parts.Count > other._parts.Count ? _parts.Count : other._parts.Count;
                string x, y;

                for (var i = 0; i < count; i++)
                {
                    x = i < _prerelease.Count ? _prerelease[i] : "";
                    y = i < other._prerelease.Count ? other._prerelease[i] : "";

                    if (x != y)
                    {
                        result = x.CompareTo(y);
                        return true;
                    }
                }
            }
            else if (IsPrerelease)
            {
                result = -1;
                return true;
            }
            else if (other.IsPrerelease)
            {
                result = 1;
                return true;
            }

            result = int.MinValue;
            return false;
        }

        private void SetParts(string input)
        {
            foreach (var part in input.Split('.'))
            {
                _parts.Add(ulong.Parse(part));
            }
        }
    }
}
