//
//  PartnerSp.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Characters;

namespace NosSmooth.Game.Data.Mates;

public record PartnerSp
(
    long VNum,
    byte? AgilityPercentage,
    PartnerSkill? Skill1,
    PartnerSkill? Skill2,
    PartnerSkill? Skill3
);