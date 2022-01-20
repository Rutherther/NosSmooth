//
//  ChatCommandsOptions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.ChatCommands;

/// <summary>
/// Options for <see cref="ChatCommandInterceptor"/>.
/// </summary>
public class ChatCommandsOptions
{
    /// <summary>
    /// Gets or sets the command prefix.
    /// </summary>
    public string Prefix { get; set; } = "#";
}