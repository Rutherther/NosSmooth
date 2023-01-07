//
//  GroupMemberStatEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Social;

namespace NosSmooth.Game.Events.Groups;

/// <summary>
/// A new stats (hp, mp) of a group received.
/// </summary>
/// <param name="Member">The updated group member.</param>
public record GroupMemberStatEvent(GroupMember Member) : IGameEvent;