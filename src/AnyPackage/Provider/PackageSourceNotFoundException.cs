// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Runtime.Serialization;

using AnyPackage.Resources;

namespace AnyPackage.Provider;

/// <summary>
/// The <c>PackageSourceNotFoundException</c> class.
/// </summary>
[Serializable]
public class PackageSourceNotFoundException : PackageProviderException
{
    /// <summary>
    /// Gets the source name.
    /// </summary>
    public string? SourceName { get; }

    /// <summary>
    /// Gets the exception message.
    /// </summary>
    public override string Message
    {
        get
        {
            return string.IsNullOrEmpty(SourceName)
            ? base.Message
            : string.Format(Strings.SourceNotFoundName, base.Message, SourceName);
        }
    }

    /// <summary>
    /// Instantiates the <c>PackageSourceNotFoundException</c> class.
    /// </summary>
    public PackageSourceNotFoundException() : base(Strings.SourceNotFound) { }

    /// <summary>
    /// Instantiates the <c>PackageSourceNotFoundException</c> class.
    /// </summary>
    /// <param name="sourceName">Specifies the source name.</param>
    public PackageSourceNotFoundException(string? sourceName) : base(Strings.SourceNotFound)
    {
        SourceName = sourceName;
    }

    /// <summary>
    /// Instantiates the <c>PackageSourceNotFoundException</c> class.
    /// </summary>
    /// <param name="sourceName">Specifies the source name.</param>
    /// <param name="message">Specifies the message.</param>
    public PackageSourceNotFoundException(string? sourceName, string? message) : base(message)
    {
        SourceName = sourceName;
    }

    /// <summary>
    /// Instantiates the <c>PackageSourceNotFoundException</c> class.
    /// </summary>
    /// <param name="message">Specifies the message.</param>
    /// <param name="innerException">Specifies the inner exception.</param>
    public PackageSourceNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }

    /// <summary>
    /// Instantiates the <c>PackageSourceNotFoundException</c> class.
    /// </summary>
    /// <param name="info">Serialized info.</param>
    /// <param name="context">Streaming context.</param>
#if NET8_0_OR_GREATER
    [Obsolete(DiagnosticId = "SYSLIB0051")]
#endif
    protected PackageSourceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        SourceName = info.GetString(nameof(SourceName));
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
        info.AddValue(nameof(SourceName), SourceName, typeof(string));
    }
}
