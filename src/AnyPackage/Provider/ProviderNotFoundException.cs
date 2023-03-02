// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Runtime.Serialization;

namespace AnyPackage.Provider
{
    /// <summary>
    /// The <c>PackageProviderNotFoundException</c> class.
    /// </summary>
    [Serializable]
    public class PackageProviderNotFoundException : PackageProviderException
    {
        // TODO: Put into string resource.
        private const string DefaultMessage = "Package provider not found."; 

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
                var message = base.Message;

                if (!string.IsNullOrEmpty(ProviderName))
                {
                    message += $" (Provider '{ProviderName}')";
                }

                return message;
            }
        }

        /// <summary>
        /// Instantiates the <c>PackageProviderNotFoundException</c> class.
        /// </summary>
        public PackageProviderNotFoundException() : base(DefaultMessage) { }

        /// <summary>
        /// Instantiates the <c>PackageProviderNotFoundException</c> class.
        /// </summary>
        /// <param name="providerName">Specifies the provider name.</param>
        public PackageProviderNotFoundException(string? providerName) : base(DefaultMessage)
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
        protected PackageProviderNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ProviderName = info.GetString(nameof(ProviderName));
        }

        /// <summary>
        /// Deserializes the properties.
        /// </summary>
        /// <param name="info">Serialized info.</param>
        /// <param name="context">Streaming context.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ProviderName), ProviderName, typeof(string));
        }
    }
}
