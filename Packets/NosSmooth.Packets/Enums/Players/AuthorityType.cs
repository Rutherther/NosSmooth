//
//  AuthorityType.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Packets.Enums.Players;

/// <summary>
/// Authority of a player.
/// </summary>
public enum AuthorityType
{
    /// <summary>
    /// The player is a regular user.
    /// </summary>
    User = 0,

    /// <summary>
    /// The player is a moderator.
    /// </summary>
    Moderator = 1,

    /// <summary>
    /// The player is a game master.
    /// </summary>
    /// <remarks>
    /// Has GM in front of the name.
    /// </remarks>
    GameMaster = 2,

    /// <summary>
    /// The player is an administrator.
    /// </summary>
    /// <remarks>
    /// Has GM in front of the name.
    /// </remarks>
    Administrator = 3,

    /// <summary>
    /// The player is a root.
    /// </summary>
    /// <remarks>
    /// Has GM in front of the name.
    /// </remarks>
    Root = 4
}