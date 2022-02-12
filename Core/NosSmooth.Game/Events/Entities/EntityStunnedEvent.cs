//
//  EntityStunnedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Entities;

namespace NosSmooth.Game.Events.Entities;

/// <summary>
/// The given entity has been stunned or unstunned.
/// </summary>
/// <param name="Entity">The entity.</param>
/// <param name="CantMove">Whether the entity cannot move.</param>
/// <param name="CantAttack">Whether the entity cannot attack.</param>
public record EntityStunnedEvent(ILivingEntity Entity, bool CantMove, bool CantAttack)
    : IGameEvent;