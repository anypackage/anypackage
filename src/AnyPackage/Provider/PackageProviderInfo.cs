// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using static AnyPackage.Provider.PackageProviderManager;

namespace AnyPackage.Provider
{
    /// <summary>
    /// This class contains information about a package provider.
    /// </summary>
    public class PackageProviderInfo
    {
        /// <summary>
        /// Gets the name of a provider.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the provider type.
        /// </summary>
        public Type ImplementingType { get; }

        /// <summary>
        /// Gets the provider identifier.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the package provider PowerShell module information.
        /// </summary>
        // TODO: Figure out if will be supporting non-module shipped providers.
        // This will determine if this property is nullable or not.
        public PSModuleInfo? Module
        {
            get
            {
                if (!_moduleRead)
                {
                    _module = GetModuleInfo(_moduleName);
                    _moduleRead = true;
                }

                return _module;
            }
        }

        /// <summary>
        /// Gets the package provider PowerShell module.
        /// </summary>
        public string? ModuleName => Module?.Name ?? _moduleName;

        /// <summary>
        /// Gets the provider full name.
        /// </summary>
        /// <remarks>
        /// The provider full name is the module name and provider name.
        /// For example, AnyPackage\NuGet
        /// If the module name is null returns the provider name.
        /// </remarks>
        public string FullName => string.IsNullOrEmpty(_moduleName) ? Name : $"{_moduleName}\\{Name}";

        /// <summary>
        /// Gets the package provider version using the module's version.
        /// </summary>
        public Version? Version => Module?.Version;

        /// <summary>
        /// Gets the package operations the provider supports.
        /// </summary>
        public PackageProviderOperations Operations { get; }

        /// <summary>
        /// Gets and sets the package provider priority.
        /// </summary>
        /// <remarks>
        /// Lower the number the higher the priority.
        /// </remarks>
        public byte Priority { get; set; } = 100;

        /// <summary>
        /// Gets if the package provider supports package by name parameter set.
        /// </summary>
        /// <remarks>
        /// If <c>false</c> package provider doesn't support
        /// <c>Name</c> parameter set for the following cmdlets:
        /// Find-Package, Install-Package, Update-Package
        /// </remarks>
        public bool PackageByName { get; }

        /// <summary>
        /// Gets if the package provider supports package by path parameter set.
        /// </summary>
        public bool PackageByFile => _fileExtensions.Count > 0;

        /// <summary>
        /// Gets supported file extensions.
        /// </summary>
        public IEnumerable<string> FileExtensions => _fileExtensions;

        /// <summary>
        /// Gets supported Uri schemes.
        /// </summary>
        public IEnumerable<string> UriSchemes => _uriSchemes;

        private HashSet<string> _fileExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private HashSet<string> _uriSchemes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private string? _moduleName;
        private PSModuleInfo? _module;
        private bool _moduleRead;

        internal PackageProviderInfo(Type type)
        {
            if (!type.IsSubclassOf(typeof(PackageProvider)))
            {
                throw new ArgumentException($"Type '{type}' does not derive from '{nameof(PackageProvider)}' class.", nameof(type));
            }

            ImplementingType = type;
            Operations = GetOperations();
            var attr = GetProviderAttribute();
            Name = attr.Name;
            PackageByName = attr.PackageByName;
            _fileExtensions = new HashSet<string>(attr.FileExtensions);
            _uriSchemes = new HashSet<string>(attr.UriSchemes);
        }

        internal PackageProviderInfo(Guid id, Type type) : this(type)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("The value cannot be an empty GUID.", nameof(id));
            }

            Id = id;
        }

        internal PackageProviderInfo(string name, PSModuleInfo module)
        {
            Name = name;
            _module = module;
            _moduleName = module.Name;
            _moduleRead = true;
            ImplementingType = null!;
        }

        internal PackageProviderInfo(Guid id, Type type, PSModuleInfo module) : this(id, type)
        {
            _module = module;
            _moduleName = module.Name;
            _moduleRead = true;
        }

        internal PackageProviderInfo(Guid id, Type type, string moduleName) : this(id, type)
        {
            _moduleName = moduleName;
        }

        /// <summary>
        /// Constructor for the PackageProviderInfo class.
        /// </summary>
        /// <param name="providerInfo">The package provider information to copy to this instance.</param>
        protected PackageProviderInfo(PackageProviderInfo providerInfo)
        {
            ImplementingType = providerInfo.ImplementingType;
            Priority = providerInfo.Priority;
            Id = providerInfo.Id;
            Operations = providerInfo.Operations;
            Name = providerInfo.Name;
            PackageByName = providerInfo.PackageByName;
            _module = providerInfo.Module;
            _moduleRead = true;
            _moduleName = providerInfo.ModuleName;
            _fileExtensions = new HashSet<string>(providerInfo.FileExtensions);
            _uriSchemes = new HashSet<string>(providerInfo.UriSchemes);
        }

        /// <summary>
        /// Returns provider name.
        /// </summary>
        public override string ToString() => Name;

        internal PackageProvider CreateInstance()
        {
            var providerInstance = Activator.CreateInstance(ImplementingType) as PackageProvider;

            if (providerInstance is null)
            {
                throw new InvalidOperationException();
            }

            providerInstance.ProviderInfo = this;

            return providerInstance;
        }

        internal bool HasOperation(PackageProviderOperations operations)
        {
            return Operations.HasFlag(operations);
        }

        internal bool IsMatch(string name)
        {
            var wildcard = new WildcardPattern(name, WildcardOptions.IgnoreCase);
            return wildcard.IsMatch(FullName) || wildcard.IsMatch(Name);
        }

        private PackageProviderAttribute GetProviderAttribute()
        {
            var attrs = ImplementingType.GetCustomAttributes(typeof(PackageProviderAttribute), false);
            var packageProviderAttribute = attrs as PackageProviderAttribute[];

            if (packageProviderAttribute?.Length != 1)
            {
                throw new InvalidOperationException("Package provider attribute not present.");
            }

            return packageProviderAttribute[0];
        }

        private PackageProviderOperations GetOperations()
        {
            var interfaces = ImplementingType.GetInterfaces();
            var operations = PackageProviderOperations.None;

            foreach (var @interface in interfaces)
            {
                // Take interface name (IInstallPackage) and remove 'I' and 'Package' to return Install.
                var name = Regex.Replace(@interface.Name.Substring(1), $"Package", "");

                if (Enum.TryParse(name, out PackageProviderOperations operation))
                {
                    operations |= operation;
                }
            }

            return operations;
        }
    }
}
