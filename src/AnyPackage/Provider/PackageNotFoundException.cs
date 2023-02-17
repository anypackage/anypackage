// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Runtime.Serialization;

namespace AnyPackage.Provider
{
    [Serializable]
    public class PackageNotFoundException : PackageProviderException
    {
        // TODO: Put into string resource.
        private const string DefaultMessage = "Package not found."; 
        
        public string? Package { get; }

        public override string Message
        {
            get
            {
                var message = base.Message;

                if (!string.IsNullOrEmpty(Package))
                {
                    message += $" (Package '{Package}')";
                }

                return message;
            }
        }

        public PackageNotFoundException() : base(DefaultMessage) { }

        public PackageNotFoundException(string? packageName) : base(DefaultMessage)
        {
            Package = packageName;
        }

        public PackageNotFoundException(string? packageName, string? message) : base(message)
        {
            Package = packageName;
        }

        public PackageNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }

        protected PackageNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Package = info.GetString(nameof(Package));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Package), Package, typeof(string));
        }
    }
}
