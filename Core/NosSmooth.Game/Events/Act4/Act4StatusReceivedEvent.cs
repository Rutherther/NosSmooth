//
//  Act4StatusReceivedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Act4;
using NosSmooth.Packets.Enums.Entities;

namespace NosSmooth.Game.Events.Act4;

/// <summary>
/// A new act4 status was received.
/// </summary>
/// <param name="Faction">The faction the player belongs to.</param>
/// <param name="MinutesUntilReset">Minutes until the faction with raid will be reset.</param>
/// <param name="AngelStatus">The angel status.</param>
/// <param name="DemonStatus">The demon status.</param>
public record Act4StatusReceivedEvent
(
    FactionType Faction,
    long MinutesUntilReset,
    Act4FactionStatus AngelStatus,
    Act4FactionStatus DemonStatus
) : IGameEvent;