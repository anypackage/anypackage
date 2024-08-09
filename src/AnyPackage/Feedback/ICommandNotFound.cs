// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

namespace AnyPackage.Feedback;

/// <summary>
/// Interface to support command not found feedback provider.
/// </summary>
public interface ICommandNotFound
{
    /// <summary>
    /// Get packages that ship the command.
    /// </summary>
    /// <param name="context">The command not found context.</param>
    /// <param name="token">Token if the user requests the search to be cancelled.</param>
    /// <returns>Returns command not found package install information.</returns>
    IEnumerable<CommandNotFoundFeedback>? FindPackage(CommandNotFoundContext context, CancellationToken token);
}
