//
//  LevPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Attributes;

namespace NosSmooth.Packets.Packets.Server.Players;

/// <summary>
/// Sent on map change.
/// </summary>
/// <remarks>
/// Contains information about the playing character.
/// </remarks>
/// <param name="Level">The level of the player.</param>
/// <param name="LevelXp">The xp in level. TODO</param>
/// <param name="JobLevel">The job level.</param>
/// <param name="JobLevelXp">The xp in job level. TODO</param>
/// <param name="XpLoad">Unknown TODO</param>
/// <param name="JobXpLoad">Unknown TODO</param>
/// <param name="Reputation">The reputation of the player.</param>
/// <param name="SkillCp">The skill cp. (Used for learning skills)</param>
/// <param name="HeroXp">The xp in hero level. TODO</param>
/// <param name="HeroLevel">The hero level. (shown as (+xx))</param>
/// <param name="HeroXpLoad">Unknown TODO</param>
[PacketHeader("lev", PacketSource.Server)]
[GenerateSerializer]
public record LevPacket
(
    [PacketIndex(0)]
    byte Level,
    [PacketIndex(1)]
    long LevelXp,
    [PacketIndex(2)]
    byte JobLevel,
    [PacketIndex(3)]
    long JobLevelXp,
    [PacketIndex(4)]
    long XpLoad,
    [PacketIndex(5)]
    long JobXpLoad,
    [PacketIndex(6)]
    long Reputation,
    [PacketIndex(7)]
    int SkillCp,
    [PacketIndex(8)]
    long HeroXp,
    [PacketIndex(9)]
    byte HeroLevel,
    [PacketIndex(10)]
    long HeroXpLoad
) : IPacket;