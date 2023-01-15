//
//  Raid.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Social;
using NosSmooth.Packets.Enums.Raids;

namespace NosSmooth.Game.Data.Raids;

/// <summary>
/// Represents nostale raid.
/// </summary>
public record Raid
(
    RaidType Type,
    RaidState State,
    short MinimumLevel,
    short MaximumLevel,
    long? LeaderId,
    GroupMember? Leader,
    RaidProgress? Progress,
    Monster? Boss,
    IReadOnlyList<Monster>? Bosses,
    IReadOnlyList<GroupMember>? Members
);