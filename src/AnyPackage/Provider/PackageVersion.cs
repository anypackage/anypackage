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
        private static readonly Regex s_semVer = new Regex(@"^(?<major>0|[1-9]\d*)\.(?<minor>0|[1-9]\d*)\.(?<patch>0|[1-9]\d*)(?:-(?<prerelease>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?<buildMetadata>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$");
        private static readonly Regex s_multiPartNumericSuffix = new Regex(@"^(?<version>((\d+\.)+\d+))(?<suffix>[a-zA-Z]\w*)$");
        private static readonly Regex s_multiPartNumeric = new Regex(@"^(\d+\.)+\d+$");

        /// <summary>
        /// Gets the version.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Gets the dot separated first position.
        /// </summary>
        public int? Major => _parts.Count > 0 ? _parts[0] : null;

        /// <summary>
        /// Gets the dot separated second position.
        /// </summary>
        public int? Minor => _parts.Count > 1 ? _parts[1] : null;

        /// <summary>
        /// Gets the dot separated third position.
        /// </summary>
        public int? Patch => _parts.Count > 2 ? _parts[2] : null;

        /// <summary>
        /// Gets the dot separated fourth position. 
        /// </summary>
        public int? Revision => _parts.Count > 3 ? _parts[3] : null;

        /// <summary>
        /// Gets all the dot separated values.
        /// </summary>
        public IEnumerable<int> Parts => _parts;

        /// <summary>
        /// Gets if the version is a prerelease.
        /// </summary>
        public bool IsPrerelease => _prerelease.Count > 0;

        /// <summary>
        /// Gets if the version contains a suffix.
        /// </summary>
        public bool HasSuffix => Suffix is not null;

        /// <summary>
        /// Gets the suffix of a multi-part numeric with suffix version.
        /// </summary>
        public string? Suffix { get; }

        /// <summary>
        /// Gets if there
        /// </summary>
        public bool HasBuildMetadata => _buildMetadata.Count > 0;

        /// <summary>
        /// Gets the dot separated values of the prerelease string.
        /// </summary>
        public IEnumerable<string> Prerelease => _prerelease;

        /// <summary>
        /// Gets the dot separated values of the build metadata string.
        /// </summary>
        public IEnumerable<string> BuildMetadata => _buildMetadata;

        /// <summary>
        /// Gets the version scheme.
        /// </summary>
        public PackageVersionScheme Scheme { get; }

        private List<int> _parts = new List<int>();
        private List<string> _prerelease = new List<string>();
        private List<string> _buildMetadata = new List<string>();

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
                _parts.Add(int.Parse(match.Groups[1].Value));
                _parts.Add(int.Parse(match.Groups[2].Value));
                _parts.Add(int.Parse(match.Groups[3].Value));

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
                            _buildMetadata.Add(section);
                        }
                    }
                    else
                    {
                        _buildMetadata.Add(buildMetadata);
                    }
                }

                Scheme = PackageVersionScheme.SemanticVersion;
            }
            else if (s_multiPartNumericSuffix.IsMatch(version))
            {
                match = s_multiPartNumericSuffix.Match(version);
                SetDigits(match.Groups[match.Groups.Count - 2].Value);
                Suffix = match.Groups[match.Groups.Count - 1].Value;
                Scheme = PackageVersionScheme.MultiPartNumericSuffix;
            }
            else if (s_multiPartNumeric.IsMatch(version))
            {
                match = s_multiPartNumeric.Match(version);
                SetDigits(match.Captures[0].Value);
                Scheme = PackageVersionScheme.MultiPartNumeric;
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
            _parts.Add(version.Major);
            _parts.Add(version.Minor);

            if (version.Build != -1) { _parts.Add(version.Build); }
            if (version.Revision != -1) { _parts.Add(version.Revision); }
        }

        private void SetDigits(string input)
        {
            foreach (var digit in input.Split('.'))
            {
                _parts.Add(int.Parse(digit));
            }
        }

        /// <summary>
        /// Implicit cast operator for casing <c>Version</c> to <c>PackageVersion</c>.
        /// </summary>
        /// <param name="version">The Version object to convert to PackageVersion.</param>
        /// <returns>The corresponding PackageVersion object.</returns>
        public static implicit operator PackageVersion(Version version) => new PackageVersion(version);

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
        public static PackageVersion Parse(string version) => new PackageVersion(version);

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

            if (IsPrerelease)
            {
                throw new InvalidOperationException("Version contains prerelease.");
            }

            if (HasBuildMetadata)
            {
                throw new InvalidOperationException("Version contains build metadata.");
            }

            return System.Version.Parse(Version);
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
