//
//  EntityDiedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Entities;

namespace NosSmooth.Game.Events.Entities;

/// <summary>
/// An entity has died.
/// </summary>
/// <remarks>
/// Is not emitted for the character, see <see cref="CharacterDiedEvent"/>.
/// TODO figure CharacterDiedEvent out.
/// </remarks>
/// <param name="Entity">The entity that has died.</param>
/// <param name="KillSkill">The skill that was used to kill the entity, if known.</param>
public record EntityDiedEvent(ILivingEntity Entity, Skill? KillSkill) : IGameEvent;