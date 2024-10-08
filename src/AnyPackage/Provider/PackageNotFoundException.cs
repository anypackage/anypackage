// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Runtime.Serialization;

using AnyPackage.Resources;

namespace AnyPackage.Provider;

/// <summary>
/// The <c>PackageNotFoundException</c> class.
/// </summary>
[Serializable]
public class PackageNotFoundException : PackageProviderException
{
    /// <summary>
    /// Gets the package name.
    /// </summary>
    public string? Package { get; }

    /// <summary>
    /// Gets the exception message.
    /// </summary>
    public override string Message
    {
        get
        {
            return string.IsNullOrEmpty(Package)
            ? base.Message
            : string.Format(Strings.PackageNotFoundName, base.Message, Package);
        }
    }

    /// <summary>
    /// Instantiates a <c>PackageNotFoundException</c> class.
    /// </summary>
    public PackageNotFoundException() : base(Strings.PackageNotFound) { }

    /// <summary>
    /// Instantiates a <c>PackageNotFoundException</c> class.
    /// </summary>
    /// <param name="packageName">Specifies the package name.</param>
    public PackageNotFoundException(string? packageName) : base(Strings.PackageNotFound)
    {
        Package = packageName;
    }

    /// <summary>
    /// Instantiates a <c>PackageNotFoundException</c> class.
    /// </summary>
    /// <param name="packageName">Specifies the package name.</param>
    /// <param name="message">Specifies the message.</param>
    public PackageNotFoundException(string? packageName, string? message) : base(message)
    {
        Package = packageName;
    }

    /// <summary>
    /// Instantiates a <c>PackageNotFoundException</c> class.
    /// </summary>
    /// <param name="message">Specifies the message.</param>
    /// <param name="innerException">Specifies the inner exception.</param>
    public PackageNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }

    /// <summary>
    /// Instantiates a <c>PackageNotFoundException</c> class.
    /// </summary>
    /// <param name="info">Serialized info.</param>
    /// <param name="context">Streaming context.</param>
#if NET8_0_OR_GREATER
    [Obsolete(DiagnosticId = "SYSLIB0051")]
#endif
    protected PackageNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Package = info.GetString(nameof(Package));
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
        info.AddValue(nameof(Package), Package, typeof(string));
    }
}
