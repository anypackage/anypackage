// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Runtime.Serialization;

namespace AnyPackage.Provider;

/// <summary>
/// The <c>PackageProviderException</c> class.
/// </summary>
[Serializable]
public class PackageProviderException : Exception
{
    /// <summary>
    /// Instantiates the <c>PackageProviderException</c> class.
    /// </summary>
    public PackageProviderException() { }

    /// <summary>
    /// Instantiates the <c>PackageProviderException</c> class.
    /// </summary>
    /// <param name="message">Specifies the message.</param>
    public PackageProviderException(string? message) : base(message) { }

    /// <summary>
    /// Instantiates the <c>PackageProviderException</c> class.
    /// </summary>
    /// <param name="message">Specifies the message.</param>
    /// <param name="innerException">Specifies the inner exception.</param>
    /// <returns></returns>
    public PackageProviderException(string? message, Exception? innerException) : base(message, innerException) { }

    /// <summary>
    /// Instantiates the <c>PackageProviderException</c> class.
    /// </summary>
    /// <param name="info">Serialized info.</param>
    /// <param name="context">Streaming context.</param>
    #if NET8_0_OR_GREATER
    [Obsolete(DiagnosticId = "SYSLIB0051")]
    #endif
    protected PackageProviderException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
