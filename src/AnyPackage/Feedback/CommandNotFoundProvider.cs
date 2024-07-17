// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

#if NET8_0_OR_GREATER
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Subsystem;
using System.Management.Automation.Subsystem.Feedback;
using System.Management.Automation.Subsystem.Prediction;
using AnyPackage.Provider;
using AnyPackage.Resources;

namespace AnyPackage.Feedback;

/// <summary>
/// The <c>CommandNotFoundProvider</c> class.
/// The AnyPackage command not found feedback provider.
/// </summary>
public sealed class CommandNotFoundProvider : IFeedbackProvider,
                                              ICommandPredictor,
                                              IModuleAssemblyInitializer,
                                              IModuleAssemblyCleanup
{
    /// <summary>
    /// Feedback provider ID.
    /// </summary>
    public Guid Id => new("4640f573-1a93-43f0-8ccd-492a54df3b2d");
    
    /// <summary>
    /// Feedback provider name.
    /// </summary>
    public string Name => Strings.AnyPackage;
    
    /// <summary>
    /// Feedback provider description.
    /// </summary>
    public string Description => Strings.FeedbackDescription;
    
    /// <summary>
    /// PowerShell functions to define in the global scope.
    /// </summary>
    public Dictionary<string, string>? FunctionsToDefine => null;

    /// <summary>
    /// Triggers for feedback provider.
    /// </summary>
    public FeedbackTrigger Trigger => FeedbackTrigger.CommandNotFound;

    private readonly List<string> _candidates = [];
    private readonly PowerShell _powershell = PowerShell.Create(RunspaceMode.CurrentRunspace);
    private string _command = "";
    private CancellationToken _token;

    /// <see href="link">https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.subsystem.feedback.ifeedbackprovider.getfeedback</see>
    public FeedbackItem? GetFeedback(FeedbackContext context, CancellationToken token)
    {
        _command = ((CommandNotFoundException)context.LastError!.Exception).CommandName;
        _token = token;
        return new FeedbackItem(Strings.FeedbackHeader, GetActions().ToList());
    }

    /// <see href="link">https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.subsystem.feedback.ifeedbackprovider.getfeedback</see>
    public SuggestionPackage GetSuggestion(PredictionClient client, PredictionContext context, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <see href="link">https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.subsystem.prediction.commandprediction.onsuggestionaccepted</see>
    public void OnSuggestionAccepted(PredictionClient client, uint session, string acceptedSuggestion) => _candidates.Clear();

    /// <see href="link">https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.imoduleassemblyinitializer.onimport</see>
    public void OnImport()
    {
        SubsystemManager.RegisterSubsystem(SubsystemKind.FeedbackProvider, this);
        SubsystemManager.RegisterSubsystem(SubsystemKind.CommandPredictor, this);
    }

    /// <see href="link">https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.imoduleassemblycleanup.onremove</see>
    public void OnRemove(PSModuleInfo psModuleInfo)
    {
        SubsystemManager.UnregisterSubsystem<ICommandPredictor>(Id);
        SubsystemManager.UnregisterSubsystem<IFeedbackProvider>(Id);
    }

    private IEnumerable<string> GetActions()
    {
        foreach (var package in GetPackages())
        {
            yield return string.Format(Strings.FeedbackWithProvider, package.Name, package.Provider);
        }
    }

    private List<CommandNotFoundFeedback> GetPackages()
    {
        var context = new CommandNotFoundContext(_command);
        var feedback = new List<CommandNotFoundFeedback>();

        foreach (var provider in GetPackageProviders())
        {
            if (_token.IsCancellationRequested) { return feedback; }

            if (Activator.CreateInstance(provider.ImplementingType) is ICommandNotFound instance)
            {
                var packages = instance.FindPackage(context, _token);

                if (packages is not null)
                {
                    feedback.AddRange(packages);
                }
            }
        }

        return feedback;
    }

    private Collection<PackageProviderInfo> GetPackageProviders()
    {
        return _powershell.AddCommand("Get-PackageProvider")
                          .Invoke<PackageProviderInfo>();
    }
}
#endif
