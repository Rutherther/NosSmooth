//
//  GroundItem.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Game.Data.Info;
using NosSmooth.Packets.Enums;

namespace NosSmooth.Game.Data.Entities;

/// <summary>
/// The item on the ground.
/// </summary>
/// <param name="Id">The id of the item entity.</param>
/// <param name="ItemVNum">The vnum of the dropped item.</param>
/// <param name="Position">The position of the ground item.</param>
public record GroundItem(long Id, long ItemVNum, Position? Position) : IEntity
{
    /// <inheritdoc />
    public string? Name => null;

    /// <inheritdoc />
    public EntityType Type => EntityType.Object;
}