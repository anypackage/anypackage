// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using AnyPackage.Provider;

namespace AnyPackage.Commands.Internal
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
                _request ??= new SourceRequest(this);
                return _request;
            }
        }

        private SourceRequest? _request;

        /// <summary>
        /// Sets the request property.
        /// </summary>
        protected virtual void SetRequest()
        {
            Request.DynamicParameters = DynamicParameters;
            Request.HasWriteObject = false;
        }

        /// <summary>
        /// Sets the request property.
        /// </summary>
        /// <param name="name">Specifies the source name.</param>
        /// <param name="location">Specifies the source location.</param>
        /// <param name="trusted">Specifies if the source is trusted.</param>
        /// <param name="force">Specifies if an existing source should be overwritten.</param>
        protected virtual void SetRequest(string name, string? location = null, bool? trusted = null, bool? force = null)
        {
            SetRequest();
            Request.Name = name;
            Request.Location = location;
            Request.Trusted = trusted;
            Request.Force = force;
        }
    }
}
