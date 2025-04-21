// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using AnyPackage.Provider;

namespace AnyPackage.Commands;

internal class RestorePackageContext
{
    internal string Name { get; set; } = string.Empty;
    internal PackageVersionRange? Version { get; set; }
    internal string? Source { get; set; }
    internal bool Latest { get; set; }
    internal bool Prerelease { get; set; }
    internal string Provider { get; set; } = string.Empty;
}
