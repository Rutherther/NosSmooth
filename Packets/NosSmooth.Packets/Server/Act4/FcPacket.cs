//
//  FcPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Enums;
using NosSmooth.PacketSerializer.Abstractions.Attributes;

namespace NosSmooth.Packets.Server.Act4;

/// <summary>
/// Contains the state of the act4.
/// </summary>
/// <remarks>
/// Sent only in act4. Receiving the packet
/// will make status bar appear in the game.
/// The packet contains all information about
/// the current status such as percentage till Mukraju,
/// type of raid etc.
/// </remarks>
/// <param name="Faction">The faction of the player playing.</param>
/// <param name="MinutesUntilReset">The minutes it will take to reset.</param>
/// <param name="AngelState">The status of angel faction, see <see cref="FcSubPacket"/>.</param>
/// <param name="DemonState">The status of demon faction, see <see cref="FcSubPacket"/>.</param>
[PacketHeader("fc", PacketSource.Server)]
[GenerateSerializer(true)]
public record FcPacket(
    [PacketIndex(0)]
    FactionType Faction,
    [PacketIndex(1)]
    long MinutesUntilReset,
    [PacketIndex(2, InnerSeparator = ' ')]
    FcSubPacket AngelState,
    [PacketIndex(3, InnerSeparator = ' ')]
    FcSubPacket DemonState
) : IPacket;