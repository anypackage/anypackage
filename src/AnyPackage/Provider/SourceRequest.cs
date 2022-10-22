// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;

namespace AnyPackage.Provider
{
    /// <summary>
    /// The <c>SourceRequest</c> class is used
    /// to send information to the package provider.
    /// </summary>
    public sealed class SourceRequest : Request
    {
        /// <summary>
        /// Gets the package source name.
        /// </summary>
        public string Name { get; internal set; } = string.Empty;

        /// <summary>
        /// Gets the package source location.
        /// </summary>
        public string? Location { get; internal set; }

        /// <summary>
        /// Gets if the package source is trusted.
        /// </summary>
        public bool? Trusted { get; internal set; }

        /// <summary>
        /// Gets if the source should be overwritten.
        /// </summary>
        public bool? Force { get; internal set; }

        /// <summary>
        /// Gets the package source if passed in via <c>InputObject</c> parameter.
        /// </summary>
        public PackageSourceInfo? Source { get; internal set; }

        internal bool PassThru { get; set; }

        internal SourceRequest(PSCmdlet command) : base(command) { }

        // TODO: Add IsMatch methods

        /// <summary>
        /// Writes the package source to the pipeline.
        /// </summary>
        /// <param name="source">The package source.</param>
        public void WriteSource(PackageSourceInfo source)
        {
            HasWriteObject = true;

            if (PassThru)
            {
                Cmdlet.WriteObject(source);
            }
        }

        /// <summary>
        /// Writes the package repositories to the pipeline.
        /// </summary>
        /// <param name="source">The package repositories.</param>
        public void WriteSource(IEnumerable<PackageSourceInfo> source) => WriteSource(source);

        /// <summary>
        /// Writes the package source to the pipeline.
        /// </summary>
        /// <param name="name">The package source name.</param>
        /// <param name="location">The package source location.</param>
        /// <param name="trusted">If the package source is trusted or not.</param>
        /// <param name="metadata">Additional metadata about the package source.</param>
        public void WriteSource(string name,
                                    string location,
                                    bool trusted = false,
                                    Hashtable? metadata = null)
        {
            var source = new PackageSourceInfo(name, location, ProviderInfo!, trusted, metadata);
            WriteSource(source);
        }
    }
}