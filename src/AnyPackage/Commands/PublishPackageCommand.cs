// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Linq;
using System.Management.Automation;
using AnyPackage.Commands.Internal;
using AnyPackage.Provider;

namespace AnyPackage.Commands
{
    /// <summary>
    /// The Publish-Package command.
    /// </summary>
    [Cmdlet(VerbsData.Publish, "Package",
            SupportsShouldProcess = true,
            HelpUri = "https://go.anypackage.dev/Publish-Package")]
    [OutputType(typeof(PackageInfo))]
    public sealed class PublishPackageCommand : PackageCommandBase
    {
        private const PackageProviderOperations Publish = PackageProviderOperations.Publish;

        /// <summary>
        /// Gets or sets destination path.
        /// </summary>
        [Parameter(Mandatory = true,
            Position = 0,
            ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        [ValidateNoWildcards]
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        [Parameter(Mandatory = true,
            Position = 1)]
        [ValidateNotNullOrEmpty]
        [ValidateProvider(Publish)]
        [ArgumentCompleter(typeof(ProviderArgumentCompleter))]
        public override string Provider { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        [Parameter()]
        [ValidateNotNullOrEmpty]
        [Alias("Repository")]
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets if the command should pass objects through.
        /// </summary>
        [Parameter]
        public SwitchParameter PassThru { get; set; }

        /// <summary>
        /// Instantiates the <c>PublishPackageCommand</c> class.
        /// </summary>
        public PublishPackageCommand()
        {
            Operation = Publish;
        }

        /// <summary>
        /// Processes input.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (!ShouldProcess(Path))
            {
                return;
            }

            var instance = GetInstances(Provider).First();

            WriteVerbose($"Publishing '{Path}' package.");

            string? source = MyInvocation.BoundParameters.ContainsKey(nameof(Source)) ? Source : null;
            SetRequest(Path, source);

            WriteVerbose($"Calling '{instance.ProviderInfo.Name}' provider.");
            Request.ProviderInfo = instance.ProviderInfo;

            try
            {
                instance.PublishPackage(Request);
            }
            catch (PipelineStoppedException)
            {
                throw;
            }
            catch (Exception e)
            {
                var ex = new PackageProviderException(e.Message, e);
                var er = new ErrorRecord(ex, "PackageProviderError", ErrorCategory.NotSpecified, Path);
                WriteError(er);
            }

            if (!Request.HasWriteObject)
            {
                var ex = new PackageProviderException("Package provider did not publish package.");
                var err = new ErrorRecord(ex, "PackageNotPublished", ErrorCategory.NotSpecified, Path);
                WriteError(err);
            }
        }

        private void SetRequest(string path, string? source)
        {
            base.SetRequest();
            Request.Path = path;
            Request.Source = source;
            Request.PassThru = PassThru;
        }
    }
}
