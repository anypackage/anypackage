// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using AnyPackage.Provider;
using static AnyPackage.Provider.PackageProviderManager;

namespace AnyPackage.Commands
{
    /// <summary>
    /// The Provider parameter argument completer.
    /// </summary>
    public sealed class ProviderArgumentCompleter : IArgumentCompleter
    {
        /// <see href="link">https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.iargumentcompleter.completeargument</see>
        public IEnumerable<CompletionResult> CompleteArgument(string commandName,
                                                              string parameterName,
                                                              string wordToComplete,
                                                              CommandAst commandAst,
                                                              IDictionary fakeBoundParameters)
        {
            var operations = GetOperation(commandName);
            var providers = GetProviders();
            var duplicates = providers.GroupBy(x => x.Name).Where(g => g.Count() > 1).Select(y => y.Key);
            var hashSet = new HashSet<string>(duplicates);
            var completions = new List<CompletionResult>();

            foreach (var provider in providers)
            {
                var nameMatch = provider.IsMatch(wordToComplete + "*");
                var operationMatch = provider.HasOperation(operations);

                if ((operations == PackageProviderOperations.None && nameMatch) ||
                   (operationMatch && nameMatch))
                {
                    var name = hashSet.Contains(provider.Name) ? provider.FullName : provider.Name;
                    var completionText = provider.Name.Contains(" ") ? $"'{name}'" : name;

                    var completion = new CompletionResult(completionText,
                                                          name,
                                                          CompletionResultType.ParameterValue,
                                                          provider.FullName);

                    completions.Add(completion);
                }
            }

            return completions.OrderBy(x => x.CompletionText, StringComparer.OrdinalIgnoreCase);
        }

        private PackageProviderOperations GetOperation(string commandName)
        {
            return commandName switch
            {
                Constants.FindPackage => PackageProviderOperations.Find,
                Constants.GetPackage => PackageProviderOperations.Get,
                Constants.InstallPackage => PackageProviderOperations.Install,
                Constants.PublishPackage => PackageProviderOperations.Publish,
                Constants.SavePackage => PackageProviderOperations.Save,
                Constants.UninstallPackage => PackageProviderOperations.Uninstall,
                Constants.UpdatePackage => PackageProviderOperations.Update,
                Constants.GetPackageSource => PackageProviderOperations.GetSource,
                Constants.RegisterPackageSource => PackageProviderOperations.SetSource,
                Constants.SetPackageSource => PackageProviderOperations.SetSource,
                Constants.UnregisterPackageSource => PackageProviderOperations.SetSource,
                _ => PackageProviderOperations.None,
            };
        }
    }
}
