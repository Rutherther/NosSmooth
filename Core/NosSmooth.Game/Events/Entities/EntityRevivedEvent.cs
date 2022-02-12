//
//  EntityRevivedEvent.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Entities;

namespace NosSmooth.Game.Events.Entities;

/// <summary>
/// The given entity has been revived.
/// </summary>
/// <param name="Entity"></param>
public record EntityRevivedEvent(ILivingEntity Entity);