//
//  PartnerSkillsReceivedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Mates;
using NosSmooth.Game.Data.Social;

namespace NosSmooth.Game.Events.Mates;

/// <summary>
/// A partner the character owns skills were received.
/// </summary>
/// <param name="Partner">The partner.</param>
/// <param name="Skills">The skills of the partner.</param>
public record PartnerSkillsReceivedEvent(Partner? Partner, IReadOnlyList<Skill> Skills) : IGameEvent;