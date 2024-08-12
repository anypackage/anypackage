// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

namespace AnyPackage.Feedback;

/// <summary>
/// The <c>CommandNotFoundContext</c> class.
/// Contains information about command not found.
/// </summary>
/// <param name="command">The command name.</param>
public sealed class CommandNotFoundContext(string command)
{
    /// <summary>
    /// Gets the command name not found.
    /// </summary>
    public string Command { get; } = command;
}
