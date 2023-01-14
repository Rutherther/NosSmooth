//
//  RaidState.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.Game.Data.Raids;

/// <summary>
/// A state of a <see cref="Raid"/>.
/// </summary>
public enum RaidState
{
    /// <summary>
    /// Waiting for a raid start.
    /// </summary>
    Waiting,

    /// <summary>
    /// The raid has started, the current room is not boss room.
    /// </summary>
    Started,

    /// <summary>
    /// The raid has started and the current room is boss room.
    /// </summary>
    BossFight,

    /// <summary>
    /// The raid has ended, successfully.
    /// </summary>
    EndedSuccessfully,

    /// <summary>
    /// The raid has ended unsuccessfully. The whole team has failed.
    /// </summary>
    TeamFailed,

    /// <summary>
    /// The raid has ended unsuccessfully for the character. He ran out of lifes.
    /// </summary>
    MemberFailed,

    /// <summary>
    /// The character has left the raid.
    /// </summary>
    /// <remarks>
    /// The previous state is needed to be able to tell whether
    /// the raid was already started or was in the <see cref="Waiting"/> state.
    /// </remarks>
    Left
}