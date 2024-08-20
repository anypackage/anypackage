// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Collections;
using System.Collections.ObjectModel;
using AnyPackage.Provider;

namespace AnyPackage.Feedback;

/// <summary>
/// The <c>CommandNotFoundFeedback</c> class.
/// The command not found package information.
/// </summary>
/// <param name="name">Missing command package name.</param>
/// <param name="provider">Package provider.</param>
public sealed class CommandNotFoundFeedback(string name, PackageProviderInfo provider)
{
    /// <summary>
    /// Gets the missing command package name.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets if any other parameters are required to install the package.
    /// </summary>
    /// <remarks>
    /// If a specific version or package source is required add it, otherwise don't.
    /// Do not include Provider parameter as that will automatically be added.
    /// </remarks>
    public IReadOnlyDictionary<string, string>? RequiredParameters { get; }

    /// <summary>
    /// Gets the package provider info.
    /// </summary>
    public PackageProviderInfo Provider { get; } = provider;

    /// <summary>
    /// Instantiates a <c>PackageNotFoundException</c> class.
    /// </summary>
    /// <param name="name">Missing command package name.</param>
    /// <param name="provider">Package provider.</param>
    /// <param name="requiredParameters">Required parameters to install package.</param>
    public CommandNotFoundFeedback(string name, PackageProviderInfo provider, IDictionary<string, string> requiredParameters) : this(name, provider)
    {
        RequiredParameters = new ReadOnlyDictionary<string, string>(requiredParameters);
    }

    /// <summary>
    /// Instantiates a <c>PackageNotFoundException</c> class.
    /// </summary>
    /// <param name="name">Missing command package name.</param>
    /// <param name="provider">Package provider.</param>
    /// <param name="requiredParameters">Required parameters to install package.</param>
    public CommandNotFoundFeedback(string name, PackageProviderInfo provider, Hashtable requiredParameters) : this(name, provider)
    {
        if (requiredParameters.Count > 0)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (DictionaryEntry entry in requiredParameters)
            {
                if (entry.Value is null) {
                    throw new ArgumentNullException(nameof(requiredParameters));
                }
                
                var key = entry.Key.ToString();
                var value = entry.Value.ToString();

                if (key is not null && value is not null)
                {
                    dictionary.Add(key, value);
                }
            }

            RequiredParameters = new ReadOnlyDictionary<string, string>(dictionary);
        }
    }
}
