// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using UniversalPackageManager.Provider;

namespace UniversalPackageManager.Commands.Internal
{
    /// <summary>
    /// The base class for source commands.
    /// </summary>
    public abstract class SourceCommandBase : CommandBase
    {
        /// <summary>
        /// Gets or sets the package request.
        /// </summary>
        protected SourceRequest Request
        {
            get
            {
                _request = _request ?? new SourceRequest(this);
                return _request;
            }
        }

        private SourceRequest? _request;

        protected virtual void SetRequest()
        {
            Request.DynamicParameters = DynamicParameters;
            Request.HasWriteObject = false;
        }

        protected virtual void SetRequest(string name, string? location = null, bool? trusted = null, bool? force = null)
        {
            SetRequest();
            Request.Name = name;
            Request.Location = location;
            Request.Trusted = trusted;
            Request.Force = force;
        }

        protected virtual void SetRequest(PackageSourceInfo source)
        {
            SetRequest();
            Request.Name = source.Name;
            Request.Location = source.Location;
            Request.Trusted = source.Trusted;
            Request.ProviderInfo = source.Provider;
            Request.Source = source;
        }
    }
}
