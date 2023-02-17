// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Runtime.Serialization;

namespace AnyPackage.Provider
{
    [Serializable]
    public class PackageProviderNotFoundException : PackageProviderException
    {
        // TODO: Put into string resource.
        private const string DefaultMessage = "Package provider not found."; 
        
        public string? ProviderName { get; }

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

        public PackageProviderNotFoundException() : base(DefaultMessage) { }

        public PackageProviderNotFoundException(string? providerName) : base(DefaultMessage)
        {
            ProviderName = providerName;
        }

        public PackageProviderNotFoundException(string? providerName, string? message) : base(message)
        {
            ProviderName = providerName;
        }

        public PackageProviderNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }

        protected PackageProviderNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ProviderName = info.GetString(nameof(ProviderName));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ProviderName), ProviderName, typeof(string));
        }
    }
}
