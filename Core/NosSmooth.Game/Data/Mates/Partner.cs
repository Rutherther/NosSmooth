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

/// <summary>
/// Information about player's partner
/// </summary>
/// <param name="MateId">The id of the mate.</param>
/// <param name="NpcVNum">The vnum of the mate.</param>
/// <param name="TransportId">Unknown function TODO.</param>
/// <param name="Level">The level of the mate.</param>
/// <param name="Loyalty">The loyalty of the mate.</param>
/// <param name="Attack">The attack statistics of the mate.</param>
/// <param name="Armor">The armor statistics of the mate.</param>
/// <param name="Equipment">The equipment of the partner.</param>
/// <param name="Element">The element of the mate.</param>
/// <param name="Resistance">The resistance of the mate.</param>
/// <param name="Hp">The health of the mate.</param>
/// <param name="Mp">The mana of the mate.</param>
/// <param name="MorphVNum">The morph vnum of the partner.</param>
/// <param name="Name">The name of the mate.</param>
/// <param name="IsSummonable">Whether the mate is summonable.</param>
/// <param name="Sp">The equipped sp of the partner.</param>
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