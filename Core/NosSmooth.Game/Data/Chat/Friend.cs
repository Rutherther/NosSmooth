//
//  Friend.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums.Relations;

namespace NosSmooth.Game.Data.Chat;

/// <summary>
/// Represents character's friend.
/// </summary>
public record Friend(long PlayerId, CharacterRelationType RelationType)
{
    /// <summary>
    /// The name of the character.
    /// </summary>
    public string? CharacterName { get; internal set; }

    /// <summary>
    /// Whether the friend is connected to the server.
    /// </summary>
    public bool IsConnected { get; internal set; }
}