// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;

namespace AnyPackage.Provider
{
    /// <summary>
    /// Indicates the package provider doesn't support
    /// <c>Name</c> parameter set.
    /// </summary>
    /// <remarks>
    /// Used for providers that use <c>Path</c> parameter set.
    /// Used for the following cmdlets:
    /// Find-Package, Install-Package, Update-Package
    /// </remarks>
    public sealed class NoPackageByNameAttribute : Attribute { }
}