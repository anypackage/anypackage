// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;

namespace UniversalPackageManager.Provider
{
    [Flags]
    public enum PackageProviderOperations
    {
        None = 0,
        Find = 1,
        Get = 2,
        Publish = 4,
        Install = 8,
        Save = 16,
        Uninstall = 32,
        Unpublish = 64,
        Update = 128,
        GetSource = 256,
        SetSource = 512
        // potential additions new, build, restore (sync), optimize (remove outdated)
    }
}
