// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

namespace AnyPackage.Feedback;

/// <summary>
/// Interface to support PowerShell feedback provider.
/// </summary>
public interface ICommandNotFound
{
    /// <summary>
    /// Get packages that ship the command.
    /// </summary>
    /// <param name="context">The command not found context.</param>
    /// <param name="cancellationToken">Token if the user requests the search to be cancelled.</param>
    /// <returns></returns>
    IEnumerable<CommandNotFoundFeedback>? FindPackage(CommandNotFoundContext context, CancellationToken cancellationToken);
}
