// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

#if NET8_0_OR_GREATER
using System.Management.Automation;
using System.Management.Automation.Subsystem;
using System.Management.Automation.Subsystem.Feedback;
using System.Management.Automation.Subsystem.Prediction;

namespace AnyPackage.Feedback;

/// <summary>
/// The module initializer class.
/// </summary>
public class Init : IModuleAssemblyInitializer, IModuleAssemblyCleanup
{
    private readonly CommandNotFoundProvider _provider = new();

    /// <see href="link">https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.imoduleassemblyinitializer.onimport</see>
    public void OnImport()
    {
        try
        {
            SubsystemManager.RegisterSubsystem(SubsystemKind.FeedbackProvider, _provider);
            SubsystemManager.RegisterSubsystem(SubsystemKind.CommandPredictor, _provider);
            _provider.RegisterEvent();
        }
        catch (InvalidOperationException)
        {

        }
    }

    /// <see href="link">https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.imoduleassemblycleanup.onremove</see>
    public void OnRemove(PSModuleInfo psModuleInfo)
    {
        try
        {
            SubsystemManager.UnregisterSubsystem<ICommandPredictor>(_provider.Id);
            SubsystemManager.UnregisterSubsystem<IFeedbackProvider>(_provider.Id);
            _provider.Dispose();
        }
        catch (InvalidOperationException)
        {

        }
    }
}
#endif