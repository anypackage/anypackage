// Copyright (c) Thomas Nieto - All Rights Reserved
// You may use, distribute and modify this code under the
// terms of the MIT license.

namespace AnyPackage.Feedback;

/// <summary>
/// The <c>CommandNotFoundContext</c> class.
/// Contains information about command not found.
/// </summary>
/// <param name="command">The command name.</param>
class CommandNotFoundContext(string command)
{
    /// <summary>
    /// The command name not found.
    /// </summary>
    public string Command { get; } = command;
}