//
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
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>An item info or an error.</returns>
    public Task<Result<IItemInfo>> GetItemInfoAsync(int vnum, CancellationToken ct = default);

    /// <summary>
    /// Gets the information about a map.
    /// </summary>
    /// <param name="id">The identifier of the map.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A map info or an error.</returns>
    public Task<Result<IMapInfo>> GetMapInfoAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Gets the information about a monster.
    /// </summary>
    /// <param name="vnum">The vnum identifier of the monster.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A monster or an error.</returns>
    public Task<Result<IMonsterInfo>> GetMonsterInfoAsync(int vnum, CancellationToken ct = default);

    /// <summary>
    /// Gets the information about a skill.
    /// </summary>
    /// <param name="vnum">The vnum identifier of the skill.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A map or an error.</returns>
    public Task<Result<ISkillInfo>> GetSkillInfoAsync(int vnum, CancellationToken ct = default);
}