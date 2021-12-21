//
//  Friend.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosCore.Packets.Enumerations;

namespace NosSmooth.Game.Data.Chat;

/// <summary>
/// Represents character's friend.
/// </summary>
public class Friend
{
    /// <summary>
    /// The id of the character.
    /// </summary>
    public long CharacterId { get; internal set; }

    /// <summary>
    /// The type of the relation.
    /// </summary>
    public CharacterRelationType RelationType { get; internal set; }

    /// <summary>
    /// The name of the character.
    /// </summary>
    public string? CharacterName { get; internal set; }

    /// <summary>
    /// Whether the friend is connected to the server.
    /// </summary>
    public bool IsOnline { get; internal set; }
}