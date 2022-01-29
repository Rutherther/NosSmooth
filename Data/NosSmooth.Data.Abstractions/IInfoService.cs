﻿//
//  IInfoService.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Data.Abstractions.Infos;
using Remora.Results;

namespace NosSmooth.Data.Abstractions;

/// <summary>
/// Service for retrieving information about NosTale objects.
/// </summary>
public interface IInfoService
{
    /// <summary>
    /// Gets the information about an item.
    /// </summary>
    /// <param name="vnum">The vnum identifier of the item.</param>
    /// <returns>An item info or an error.</returns>
    public Result<IItemInfo> GetItemInfo(int vnum);

    /// <summary>
    /// Gets the information about a map.
    /// </summary>
    /// <param name="id">The identifier of the map.</param>
    /// <returns>A map info or an error.</returns>
    public Result<IMapInfo> GetMapInfo(int id);

    /// <summary>
    /// Gets the information about a monster.
    /// </summary>
    /// <param name="vnum">The vnum identifier of the monster.</param>
    /// <returns>A monster or an error.</returns>
    public Result<IMonsterInfo> GetMonsterInfo(int vnum);

    /// <summary>
    /// Gets the information about a skill.
    /// </summary>
    /// <param name="vnum">The vnum identifier of the skill.</param>
    /// <returns>A map or an error.</returns>
    public Result<ISkillInfo> GetSkillInfo(int vnum);
}