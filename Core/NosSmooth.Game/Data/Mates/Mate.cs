//
//  Mate.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Info;
using NosSmooth.Game.Data.Stats;
using NosSmooth.Packets.Enums;
using NosSmooth.PacketSerializer.Abstractions.Common;

namespace NosSmooth.Game.Data.Mates;

public record Mate
(
    long MateId,
    long NpcVNum,
    long TransportId,
    Level Level,
    short Loyalty,
    MateAttackStats Attack,
    MateArmorStats Armor,
    Element Element,
    Resistance Resistance,
    Health Hp,
    Health Mp,
    string Name,
    bool IsSummonable
);