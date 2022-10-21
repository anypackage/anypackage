// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Runtime.Serialization;

namespace AnyPackage.Provider
{
    [Serializable]
    public class PackageSourceNotFoundException : PackageProviderException
    {
        // TODO: Put into string resource.
        private const string DefaultMessage = "Package source not found."; 
        
        public string? SourceName { get; }

        public override string Message
        {
            get
            {
                var message = base.Message;

                if (!string.IsNullOrEmpty(SourceName))
                {
                    message += $" (Source '{SourceName}')";
                }

                return message;
            }
        }

        public PackageSourceNotFoundException() : base(DefaultMessage)
        {

        }

        public PackageSourceNotFoundException(string? sourceName) : base(DefaultMessage)
        {
            SourceName = sourceName;
        }

        public PackageSourceNotFoundException(string? sourceName, string? message) : base(message)
        {
            SourceName = sourceName;
        }

        public PackageSourceNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {

        }

        protected PackageSourceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            SourceName = info.GetString(nameof(SourceName));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(SourceName), SourceName, typeof(string));
        }
    }
}
