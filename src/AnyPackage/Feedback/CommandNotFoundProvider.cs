// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

#if NET8_0_OR_GREATER
using System.Management.Automation;
using System.Management.Automation.Subsystem.Feedback;
using System.Management.Automation.Subsystem.Prediction;
using System.Management.Automation.Runspaces;
using AnyPackage.Provider;
using AnyPackage.Resources;

namespace AnyPackage.Feedback;

/// <summary>
/// The <c>CommandNotFoundProvider</c> class.
/// The AnyPackage command not found feedback provider.
/// </summary>
public sealed class CommandNotFoundProvider : IFeedbackProvider, ICommandPredictor, IDisposable
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

    private readonly List<string> _candidates = [];
    private readonly Dictionary<Guid, Runspace> _runspaces = [];

    /// <see href="link">https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.subsystem.feedback.ifeedbackprovider.getfeedback</see>
    public FeedbackItem? GetFeedback(FeedbackContext context, CancellationToken token)
    {
        _candidates.Clear();
        var command = ((CommandNotFoundException)context.LastError!.Exception).CommandName;
        var commandNotFoundContext = new CommandNotFoundContext(command);

        foreach (var runspace in _runspaces)
        {
            if (token.IsCancellationRequested) { return null; }

            var script = @"
                param($id, $context, $token)
                $provider = Get-PackageProvider | Where-Object Id -eq $id
                $instance = $provider.CreateInstance()
                $instance.FindPackage($context, $token)
            ";

            using var ps = PowerShell.Create();
            ps.Runspace = runspace.Value;
            var results = ps.AddScript(script)
                            .AddArgument(runspace.Key)
                            .AddArgument(commandNotFoundContext)
                            .AddArgument(token)
                            .Invoke<CommandNotFoundFeedback>();

            foreach (var result in results)
            {
                _candidates.Add(GetAction(result));
            }
        }

        if (_candidates.Count > 0)
        {
            return new FeedbackItem(Strings.FeedbackHeader, _candidates);
        }
        else
        {
            return null;
        }
    }

    private static string GetAction(CommandNotFoundFeedback result)
    {
        var requiredParameters = "";

        if (result.RequiredParameters is not null)
        {
            foreach (var parameter in result.RequiredParameters)
            {
                requiredParameters += string.Format(" -{0} {1}", parameter.Key, parameter.Value);
            }

            return string.Format(Strings.FeedbackWithProviderAndParameters, result.Name, result.Provider.Name, requiredParameters);

        }

        return string.Format(Strings.FeedbackWithProvider, result.Name, result.Provider.Name);
    }

    /// <see href="link">https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.subsystem.feedback.ifeedbackprovider.getfeedback</see>
    public SuggestionPackage GetSuggestion(PredictionClient client, PredictionContext context, CancellationToken cancellationToken)
    {
        if (_candidates.Count > 0)
        {
            var input = context.InputAst.Extent.Text;
            List<PredictiveSuggestion>? result = null;

            foreach (var candidate in _candidates)
            {
                if (candidate.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                {
                    result ??= new List<PredictiveSuggestion>(_candidates.Count);
                    result.Add(new PredictiveSuggestion(candidate));
                }
            }

            if (result is not null)
            {
                return new SuggestionPackage(result);
            }
        }

        return default;
    }

    /// <see href="link">https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.subsystem.prediction.commandprediction.oncommandlineexecuted</see>
    public void OnCommandLineAccepted(PredictionClient client, IReadOnlyList<string> history) => _candidates.Clear();

    /// <see href="link">https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.subsystem.prediction.icommandpredictor.canacceptfeedback</see>
    public bool CanAcceptFeedback(PredictionClient client, PredictorFeedbackKind feedback) => feedback == PredictorFeedbackKind.CommandLineAccepted;

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        Runspace.DefaultRunspace.AvailabilityChanged -= SyncProviderRunspaces;
        StopProviders();
    }

    internal void RegisterEvent()
    {
        Runspace.DefaultRunspace.AvailabilityChanged += SyncProviderRunspaces;
    }

    private void SyncProviderRunspaces(object? sender, RunspaceAvailabilityEventArgs e)
    {
        if (sender is null || e.RunspaceAvailability != RunspaceAvailability.Available)
        {
            return;
        }

        var runspace = (Runspace)sender;
        runspace.AvailabilityChanged -= SyncProviderRunspaces;

        try
        {
            using var ps = PowerShell.Create();
            ps.Runspace = runspace;

            var providers = ps.AddCommand("Get-PackageProvider")
                              .Invoke<PackageProviderInfo>()
                              .Where(x => typeof(ICommandNotFound).IsAssignableFrom(x.ImplementingType));

            foreach (var provider in providers)
            {
                StartProvider(provider);
            }

            var ids = providers.Select(x => x.Id);
            var stopIds = _runspaces.Keys.Except(ids);

            foreach (var id in stopIds)
            {
                StopProvider(id);
            }
        }
        finally
        {
            runspace.AvailabilityChanged += SyncProviderRunspaces;
        }
    }

    private void StartProvider(PackageProviderInfo provider)
    {
        if (_runspaces.ContainsKey(provider.Id))
        {
            return;
        }

        var runspace = RunspaceFactory.CreateRunspace();
        runspace.Name = provider.FullName;
        runspace.ThreadOptions = PSThreadOptions.ReuseThread;
        runspace.Open();

        using var ps = PowerShell.Create();
        ps.Runspace = runspace;

        ps.AddCommand("Import-Module")
          .AddParameter("Name", provider.ModuleName)
          .Invoke();

        _runspaces.Add(provider.Id, runspace);
    }

    private void StopProvider(Guid id)
    {
        _runspaces[id].Dispose();
        _runspaces.Remove(id);
    }

    private void StopProviders()
    {
        foreach (var runspace in _runspaces.Values)
        {
            runspace.Dispose();
        }

        _runspaces.Clear();
    }

}
#endif
