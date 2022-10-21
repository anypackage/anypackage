// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversalPackageManager.Provider
{
    public abstract class PackageProviderBasic : PackageProvider
    {
        protected PackageProviderBasic(Guid id) : base(id)
        {
            throw new NotImplementedException();
        }

        public new virtual void FindPackage(PackageRequest request)
        {
            FindPackage(request, true);
        }

        protected abstract IEnumerable<PackageInfo> FindPackage(PackageRequest request, bool writeOutput);

        public new virtual void GetPackage(PackageRequest request)
        {
            GetPackage(request, true);
        }

        protected abstract IEnumerable<PackageInfo> GetPackage(PackageRequest request, bool writeOutput);

        public new virtual void InstallPackage(PackageRequest request)
        {
            InstallPackage(request, findBeforeInstall: true, dependencyCheck: false);
        }

        protected void InstallPackage(PackageRequest request, bool findBeforeInstall, bool dependencyCheck)
        {
            if (findBeforeInstall)
            {
                List<PackageInfo> find = FindPackage(request, false).ToList();
                
                if (find.Count == 0)
                {
                    throw new PackageNotFoundException(request.Name);
                }
            }
        }

        protected abstract IEnumerable<PackageInfo> InstallPackage(PackageRequest request, bool writeOutput);
    }
}