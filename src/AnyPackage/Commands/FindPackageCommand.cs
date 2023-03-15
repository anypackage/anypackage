// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Management.Automation;
using AnyPackage.Commands.Internal;
using AnyPackage.Provider;

namespace AnyPackage.Commands
{
    /// <summary>
    /// The Find-Package command.
    /// </summary>
    [Cmdlet(VerbsCommon.Find, "Package", HelpUri = "https://go.anypackage.dev/Find-Package")]
    [OutputType(typeof(PackageInfo))]
    public sealed class FindPackageCommand : PackageCommandBase
    {
        private const PackageProviderOperations Find = PackageProviderOperations.Find;

        /// <summary>
        /// Gets or sets the name(s).
        /// </summary>
        [Parameter(Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [SupportsWildcards]
        [ValidateNotNullOrEmpty]
        public string[] Name { get; set; } = new string[] { "*" };

        /// <summary>
        /// Gets or sets the version of the packages to retrieve.
        /// </summary>
        /// <remarks>
        /// Accepts NuGet version range syntax.
        /// </remarks>
        [Parameter(Position = 1)]
        [ValidateNotNullOrEmpty]
        public PackageVersionRange Version { get; set; } = new PackageVersionRange();

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        [ValidateNoWildcards]
        [ArgumentCompleter(typeof(SourceArgumentCompleter))]
        [Alias("Repository")]
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets if prerelease versions should be included.
        /// </summary>
        [Parameter]
        public SwitchParameter Prerelease { get; set; }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        [Parameter]
        [ValidateNotNullOrEmpty]
        [ValidateProvider(Find)]
        [ArgumentCompleter(typeof(ProviderArgumentCompleter))]
        public override string Provider { get; set; } = string.Empty;

        /// <summary>
        /// Instances the <c>FindPackageCommand</c> class.
        /// </summary>
        public FindPackageCommand()
        {
            Operation = Find;
        }

        /// <summary>
        /// Initializes the command.
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            Request.DynamicParameters = DynamicParameters;
            Request.PassThru = true;
        }

        /// <summary>
        /// Processes input.
        /// </summary>
        protected override void ProcessRecord()
        {
            var instances = GetInstances(Provider);

            PackageVersionRange? version = MyInvocation.BoundParameters.ContainsKey(nameof(Version)) ? Version : null;
            string? source = MyInvocation.BoundParameters.ContainsKey(nameof(Source)) ? Source : null;

            foreach (var name in Name)
            {
                WriteVerbose($"Finding '{name}' package.");

                SetRequest(name, version, source);

                foreach (var instance in instances)
                {
                    WriteVerbose($"Calling '{instance.ProviderInfo.Name}' provider.");
                    Request.ProviderInfo = instance.ProviderInfo;

                    try
                    {
                        instance.FindPackage(Request);
                    }
                    catch (PipelineStoppedException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        var ex = new PackageProviderException(e.Message, e);
                        var er = new ErrorRecord(ex, "PackageProviderError", ErrorCategory.NotSpecified, name);
                        WriteError(er);
                    }
                }

                if (!Request.HasWriteObject && !WildcardPattern.ContainsWildcardCharacters(name))
                {
                    var ex = new PackageNotFoundException(name);
                    var er = new ErrorRecord(ex, "PackageNotFound", ErrorCategory.ObjectNotFound, name);
                    WriteError(er);
                }
            }
        }

        /// <summary>
        /// Sets the request property.
        /// </summary>
        protected override void SetRequest()
        {
            base.SetRequest();
            Request.PassThru = true;
            Request.Prerelease = Prerelease;
        }
    }
}
