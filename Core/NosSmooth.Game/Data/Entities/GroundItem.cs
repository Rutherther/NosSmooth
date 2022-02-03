//
//  GroundItem.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Infos;
using NosSmooth.Game.Data.Info;
using NosSmooth.Packets.Enums;

namespace NosSmooth.Game.Data.Entities;

/// <summary>
/// The item on the ground.
/// </summary>
public class GroundItem : IEntity
{
    /// <summary>
    /// Gets or sets the id of the owner, if any.
    /// </summary>
    public long? OwnerId { get; set; }

    /// <summary>
    /// Gets or sets the amount of the item on the ground.
    /// </summary>
    public int Amount { get; internal set; }

    /// <summary>
    /// Gets or sets whether the item is for a quest.
    /// </summary>
    public bool IsQuestRelated { get; internal set; }

    /// <summary>
    /// Gets or sets the info about the item, if available.
    /// </summary>
    public IItemInfo? ItemInfo { get; internal set; }

    /// <summary>
    /// Gets the VNum of the npc.
    /// </summary>
    public int VNum { get; internal set; }

    /// <inheritdoc/>
    public long Id { get; set; }

    /// <inheritdoc/>
    public Position? Position { get; set; }

    /// <inheritdoc/>
    public EntityType Type
    {
        get => EntityType.Object;
    }
}