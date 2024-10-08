// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

using System.Management.Automation;

namespace AnyPackage.Provider;

/// <summary>
/// Request base class used to send information to the package provider.
/// </summary>
public abstract class Request
{
    /// <summary>
    /// Gets the package provider dynamic parameters.
    /// </summary>
    public object? DynamicParameters { get; internal set; }

    /// <summary>
    /// Gets the parameter set name.
    /// </summary>
    public string ParameterSetName => Cmdlet.ParameterSetName;

    /// <summary>
    /// Gets if the package provider has been requested to stop.
    /// </summary>
    public bool Stopping => Cmdlet.Stopping;

    /// <summary>
    /// Gets the package provider information.
    /// </summary>
    public PackageProviderInfo? ProviderInfo { get; internal set; }

    internal bool HasWriteObject { get; set; }

    internal PSCmdlet Cmdlet { get; }

    internal Request(PSCmdlet command)
    {
        Cmdlet = command;
    }

    /// <see href="link">https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.cmdlet.shouldcontinue</see>
    public bool ShouldContinue(string query, string caption) => Cmdlet.ShouldContinue(query, caption);

    /// <see href="link">https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.cmdlet.shouldcontinue</see>
    public bool ShouldContinue(string query, string caption, ref bool yesToAll, ref bool noToAll) =>
        Cmdlet.ShouldContinue(query, caption, ref yesToAll, ref noToAll);

    /// <see href="link">https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.cmdlet.shouldcontinue</see>
    public bool ShouldContinue(string query, string caption, bool hasSecurityImpact, ref bool yesToAll, ref bool noToAll) =>
        Cmdlet.ShouldContinue(query, caption, hasSecurityImpact, ref yesToAll, ref noToAll);

    /// <summary>
    /// Write debug information to the debug stream.
    /// </summary>
    /// <param name="text">Debug information.</param>
    public void WriteDebug(string text) => Cmdlet.WriteDebug(text);

    /// <summary>
    /// Write error record to the error stream.
    /// </summary>
    /// <param name="errorRecord">Error record.</param>
    public void WriteError(ErrorRecord errorRecord) => Cmdlet.WriteError(errorRecord);

    /// <summary>
    /// Route information to the user or host.
    /// </summary>
    /// <param name="messageData">The object / message data to transmit to the hosting application.</param>
    /// <param name="tags">
    /// Any tags to be associated with the message data.
    /// These can later be used to filter or separate objects being sent to the host
    /// </param>
    public void WriteInformation(object messageData, string[] tags) => Cmdlet.WriteInformation(messageData, tags);

    /// <summary>
    /// Write information to the information stream.
    /// </summary>
    /// <param name="informationRecord">Information.</param>
    public void WriteInformation(InformationRecord informationRecord) => Cmdlet.WriteInformation(informationRecord);

    /// <summary>
    /// Write progress information to the progress stream.
    /// </summary>
    /// <param name="progressRecord">Progress information.</param>
    public void WriteProgress(ProgressRecord progressRecord) => Cmdlet.WriteProgress(progressRecord);

    /// <summary>
    /// Write verbose information to the verbose stream.
    /// </summary>
    /// <param name="text">Verbose message.</param>
    public void WriteVerbose(string text) => Cmdlet.WriteVerbose(text);

    /// <summary>
    /// Write warning information to the warning stream.
    /// </summary>
    /// <param name="text">Warning information.</param>
    public void WriteWarning(string text) => Cmdlet.WriteWarning(text);
}
