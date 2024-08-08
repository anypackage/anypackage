// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Runtime.Serialization;
using AnyPackage.Resources;

namespace AnyPackage.Provider
{
    /// <summary>
    /// The <c>PackageProviderNotFoundException</c> class.
    /// </summary>
    [Serializable]
    public class PackageProviderNotFoundException : PackageProviderException
    {
        /// <summary>
        /// Gets the provider name.
        /// </summary>
        public string? ProviderName { get; }

        /// <summary>
        /// Gets the exception message.
        /// </summary>
        public override string Message
        {
            get
            {
                return string.IsNullOrEmpty(ProviderName)
                ? base.Message
                : string.Format(Strings.PackageProviderNotFoundName, base.Message, ProviderName);
            }
        }

        /// <summary>
        /// Instantiates the <c>PackageProviderNotFoundException</c> class.
        /// </summary>
        public PackageProviderNotFoundException() : base(Strings.PackageProviderNotFound) { }

        /// <summary>
        /// Instantiates the <c>PackageProviderNotFoundException</c> class.
        /// </summary>
        /// <param name="providerName">Specifies the provider name.</param>
        public PackageProviderNotFoundException(string? providerName) : base(Strings.PackageProviderNotFound)
        {
            ProviderName = providerName;
        }

        /// <summary>
        /// Instantiates the <c>PackageProviderNotFoundException</c> class.
        /// </summary>
        /// <param name="providerName">Specifies the provider name.</param>
        /// <param name="message">Specifies the message.</param>
        public PackageProviderNotFoundException(string? providerName, string? message) : base(message)
        {
            ProviderName = providerName;
        }

        /// <summary>
        /// Instantiates the <c>PackageProviderNotFoundException</c> class.
        /// </summary>
        /// <param name="message">Specifies the message.</param>
        /// <param name="innerException">Specifies the inner exception.</param>
        public PackageProviderNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }

        /// <summary>
        /// Instantiates the <c>PackageProviderNotFoundException</c> class.
        /// </summary>
        /// <param name="info">Serialized info.</param>
        /// <param name="context">Streaming context.</param>
        #if NET8_0_OR_GREATER
        [Obsolete(DiagnosticId = "SYSLIB0051")]
        #endif
        protected PackageProviderNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ProviderName = info.GetString(nameof(ProviderName));
        }

        /// <summary>
        /// Deserializes the properties.
        /// </summary>
        /// <param name="info">Serialized info.</param>
        /// <param name="context">Streaming context.</param>
        #if NET8_0_OR_GREATER
        [Obsolete(DiagnosticId = "SYSLIB0051")]
        #endif
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ProviderName), ProviderName, typeof(string));
        }
    }
}
