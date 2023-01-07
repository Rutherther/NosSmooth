//
//  Partner.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Info;
using NosSmooth.Game.Data.Items;
using NosSmooth.Game.Data.Stats;
using NosSmooth.Packets.Enums;

namespace NosSmooth.Game.Data.Mates;

public record Partner
(
    long MateId,
    long NpcVNum,
    long TransportId,
    Level Level,
    short Loyalty,
    MateAttackStats Attack,
    MateArmorStats Armor,
    PartnerEquipment Equipment,
    Element Element,
    Resistance Resistance,
    Health Hp,
    Health Mp,
    string Name,
    int? MorphVNum,
    bool IsSummonable,
    PartnerSp? Sp
) : Mate
(
    MateId,
    NpcVNum,
    TransportId,
    Level,
    Loyalty,
    Attack,
    Armor,
    Element,
    Resistance,
    Hp,
    Mp,
    Name,
    IsSummonable
);