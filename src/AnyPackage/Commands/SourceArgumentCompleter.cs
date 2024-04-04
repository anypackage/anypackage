// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using AnyPackage.Provider;

namespace AnyPackage.Commands
{
    /// <summary>
    /// The Source parameter argument completer.
    /// </summary>
    public sealed class SourceArgumentCompleter : IArgumentCompleter
    {
        private const string _provider = "Provider";

        /// <see href="link">https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.iargumentcompleter.completeargument</see>
        public IEnumerable<CompletionResult> CompleteArgument(string commandName,
                                                              string parameterName,
                                                              string wordToComplete,
                                                              CommandAst commandAst,
                                                              IDictionary fakeBoundParameters)
        {
            var powershell = PowerShell.Create(RunspaceMode.CurrentRunspace)
                                       .AddCommand("Get-PackageSource");

            if (fakeBoundParameters.Contains(_provider))
            {
                powershell.AddParameter(_provider, fakeBoundParameters[_provider]);
            }

            var names = powershell.Invoke<PackageSourceInfo>().Select(x => x.Name).Distinct();
            var wildcard = new WildcardPattern(wordToComplete + "*", WildcardOptions.IgnoreCase);

            foreach (var name in names)
            {
                if (wildcard.IsMatch(name))
                {
                    string completionText = name;

                    if (name.Contains(" "))
                    {
                        completionText = string.Format("'{0}'", CodeGeneration.EscapeSingleQuotedStringContent(name));
                    }

                    yield return new CompletionResult(completionText,
                                                      name,
                                                      CompletionResultType.ParameterValue,
                                                      name);
                }
            }
        }
    }
}
