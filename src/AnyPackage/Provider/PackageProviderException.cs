// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Runtime.Serialization;

namespace AnyPackage.Provider
{
    [Serializable]
    public class PackageProviderException : Exception
    {
        public PackageProviderException()
        {

        }

        public PackageProviderException(string? message) : base(message)
        {

        }

        public PackageProviderException(string? message, Exception? innerException) : base(message, innerException)
        {

        }

        protected PackageProviderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }    
}
