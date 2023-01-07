//
//  PetSkillReceivedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Mates;
using NosSmooth.Game.Data.Social;

namespace NosSmooth.Game.Events.Mates;

/// <summary>
/// A pet the character owns skill was received.
/// </summary>
/// <param name="Pet">The pet.</param>
/// <param name="PetSkill">The skill of the pet.</param>
public record PetSkillReceivedEvent(Pet? Pet, Skill? PetSkill) : IGameEvent;