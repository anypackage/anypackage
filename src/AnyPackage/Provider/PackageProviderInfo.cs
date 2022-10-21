// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Management.Automation;
using System.Text.RegularExpressions;

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
        public string Name
        {
            get
            {
                if (!_nameRead)
                {
                    var attrs = ImplementingType.GetCustomAttributes(typeof(PackageProviderAttribute), false);
                    var packageProviderAttribute = attrs as PackageProviderAttribute[];

                    if (packageProviderAttribute?.Length == 1)
                    {
                        _name = packageProviderAttribute[0].Name;
                        _nameRead = true;
                    }
                }

                return _name;
            }
        }

        /// <summary>
        /// Gets the provider type.
        /// </summary>
        public Type ImplementingType { get; }

        /// <summary>
        /// Gets the provider identifier.
        /// </summary>
        public Guid Id
        {
            get
            {
                if (_id == Guid.Empty)
                {
                    _id = CreateInstance().Id;
                }

                return _id;
            }

            internal set
            {
                if (value == Guid.Empty)
                {
                    throw new ArgumentException("The value cannot be an empty GUID.");
                }

                _id = value;
            }
        }

        /// <summary>
        /// Gets the package provider PowerShell module information.
        /// </summary>
        // TODO: Figure out if will be supporting non-module shipped providers.
        // This will determine if this property is nullable or not.
        public PSModuleInfo? Module { get; }

        /// <summary>
        /// Gets the package provider PowerShell module.
        /// </summary>
        public string? ModuleName
        {
            get
            {
                return Module?.Name ?? _moduleName;
            }
        }

        /// <summary>
        /// Gets the provider full name.
        /// </summary>
        /// <remarks>
        /// The provider full name is the module name and provider name.
        /// For example, AnyPackage\NuGet
        /// If the module name is null return null.
        /// </remarks>
        public string FullName
        {
            get
            {
                return $"{ModuleName}\\{Name}";
            }
        }

        /// <summary>
        /// Gets the package provider capabilities.
        /// </summary>
        public PackageProviderCapabilities Capabilities
        {
            get
            {
                if (!_capabilitiesRead)
                {
                    var type = this.ImplementingType;
                    var attrs = type.GetCustomAttributes(typeof(PackageProviderAttribute), false);
                    var packageProviderAttribute = attrs as PackageProviderAttribute[];

                    if (packageProviderAttribute?.Length == 1)
                    {
                        // TODO: Change to either remove capabilities
                        // or add logic to pull from attribute.
                        _capabilities = PackageProviderCapabilities.None;

                        _capabilitiesRead = true;
                    }
                }

                return _capabilities;
            }
        }

        /// <summary>
        /// Gets the package operations the provider supports.
        /// </summary>
        /// <remarks>
        public PackageProviderOperations Operations
        {
            get
            {
                if (!_operationsRead)
                {
                    Type type = this.ImplementingType;
                    var interfaces = type.GetInterfaces();

                    foreach (var @interface in interfaces)
                    {
                        // Take interface name (IInstallPackage) and remove 'I' and 'Package' to return Install.
                        var name = Regex.Replace(@interface.Name.Substring(1), $"Package", "");
                        var operation = PackageProviderOperations.None;

                        if (Enum.TryParse(name, out operation))
                        {
                            _operations |= operation;
                        }
                    }

                    _operationsRead = true;
                }

                return _operations;
            }
        }

        /// <summary>
        /// Gets and sets the package provider priority.
        /// </summary>
        /// <remarks>
        /// Lower the number the higher the priority.
        /// </remarks>
        public byte Priority { get; set; } = 100;

        private string _name = string.Empty;
        private string? _moduleName;
        private Guid _id;
        private PackageProviderCapabilities _capabilities = PackageProviderCapabilities.None;
        private PackageProviderOperations _operations = PackageProviderOperations.None;
        private bool _capabilitiesRead;
        private bool _operationsRead;
        private bool _nameRead;

        internal PackageProviderInfo(Type type)
        {
            ValidateType(type);
            ImplementingType = type;
        }

        internal PackageProviderInfo(Type type, PSModuleInfo module) : this(type)
        {
            Module = module;
        }

        internal PackageProviderInfo(Type type, string moduleName) : this(type)
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
            Module = providerInfo.Module;
            Priority = providerInfo.Priority;
            _id = providerInfo.Id;
            _capabilities = providerInfo.Capabilities;
            _capabilitiesRead = true;
            _operations = providerInfo.Operations;
            _operationsRead = true;
            _name = providerInfo.Name;
            _nameRead = true;
            _moduleName = providerInfo.ModuleName;
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

        internal bool HasCapability(PackageProviderCapabilities capabilities)
        {
            return Capabilities.HasFlag(capabilities);
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

        private void ValidateType(Type type)
        {
            if (!type.IsSubclassOf(typeof(PackageProvider)))
            {
                throw new ArgumentException($"Type '{type}' does not derive from '{nameof(PackageProvider)}' class.", nameof(type));
            }

            var attrs = type.GetCustomAttributes(typeof(PackageProviderAttribute), false);

            if (attrs.Length != 1)
            {
                throw new ArgumentException($"Type '{type}' does not have '{nameof(PackageProviderAttribute)}' attribute.", nameof(type));
            }
        }
    }
}
