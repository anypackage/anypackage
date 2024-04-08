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
                    string completionText = hashSet.Contains(provider.Name) ? provider.FullName : provider.Name;

                    if (provider.Name.Contains(" "))
                    {
                        completionText = $"'{completionText}'";
                    }

                    var completion = new CompletionResult(completionText,
                                                          completionText,
                                                          CompletionResultType.ParameterValue,
                                                          completionText);

                    completions.Add(completion);
                }
            }

            return completions.OrderBy(x => x.CompletionText, StringComparer.OrdinalIgnoreCase);
        }

        private PackageProviderOperations GetOperation(string commandName)
        {
            switch (commandName)
            {
                case Constants.FindPackage:
                    return PackageProviderOperations.Find;

                case Constants.GetPackage:
                    return PackageProviderOperations.Get;

                case Constants.InstallPackage:
                    return PackageProviderOperations.Install;

                case Constants.PublishPackage:
                    return PackageProviderOperations.Publish;

                case Constants.SavePackage:
                    return PackageProviderOperations.Save;

                case Constants.UninstallPackage:
                    return PackageProviderOperations.Uninstall;

                case Constants.UpdatePackage:
                    return PackageProviderOperations.Update;

                case Constants.GetPackageSource:
                    return PackageProviderOperations.GetSource;

                case Constants.RegisterPackageSource:
                    return PackageProviderOperations.SetSource;

                case Constants.SetPackageSource:
                    return PackageProviderOperations.SetSource;

                case Constants.UnregisterPackageSource:
                    return PackageProviderOperations.SetSource;

                default:
                    return PackageProviderOperations.None;
            }
        }
    }
}
