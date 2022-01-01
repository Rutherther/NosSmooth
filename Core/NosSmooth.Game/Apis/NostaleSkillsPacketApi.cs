//
//  NostaleSkillsPacketApi.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Client;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Errors;
using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Packets.Client.Battle;
using Remora.Results;

namespace NosSmooth.Game.Apis;

/// <summary>
/// Packet api for using character skills.
/// </summary>
public class NostaleSkillsPacketApi
{
    private readonly INostaleClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="NostaleSkillsPacketApi"/> class.
    /// </summary>
    /// <param name="client">The nostale client.</param>
    public NostaleSkillsPacketApi(INostaleClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Use the given (targetable) skill on specified entity.
    /// </summary>
    /// <remarks>
    /// For skills that can be used only on self, use <paramref name="entityId"/> of the character.
    /// For skills that cannot be targeted on an entity, proceed to <see cref="UseSkillAt"/>.
    /// </remarks>
    /// <param name="skillVNum">The id of the skill.</param>
    /// <param name="entityId">The id of the entity to use the skill on.</param>
    /// <param name="entityType">The type of the supplied entity.</param>
    /// <param name="mapX">The x coordinate on the map. (Used for non targeted dashes etc., says where the dash will be to.)</param>
    /// <param name="mapY">The y coordinate on the map. (Used for non targeted dashes etc., says where the dash will be to.)</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> UseSkillOn
    (
        long skillVNum,
        long entityId,
        EntityType entityType,
        short? mapX = default,
        short? mapY = default,
        CancellationToken ct = default
    )
    {
        return _client.SendPacketAsync
        (
            new UseSkillPacket
            (
                skillVNum,
                entityType,
                entityId,
                mapX,
                mapY
            ),
            ct
        );
    }

    /// <summary>
    /// Use the given (targetable) skill on specified entity.
    /// </summary>
    /// <remarks>
    /// For skills that can be used only on self, use <paramref name="entityId"/> of the character.
    /// For skills that cannot be targeted on an entity, proceed to <see cref="UseSkillAt"/>.
    /// </remarks>
    /// <param name="skillVNum">The id of the skill.</param>
    /// <param name="entity">The entity to use the skill on.</param>
    /// <param name="mapX">The x coordinate on the map. (Used for non targeted dashes etc., says where the dash will be to.)</param>
    /// <param name="mapY">The y coordinate on the map. (Used for non targeted dashes etc., says where the dash will be to.)</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> UseSkillOn
    (
        long skillVNum,
        ILivingEntity entity,
        short? mapX = default,
        short? mapY = default,
        CancellationToken ct = default
    )
    {
        return _client.SendPacketAsync
        (
            new UseSkillPacket
            (
                skillVNum,
                entity.Type,
                entity.Id,
                mapX,
                mapY
            ),
            ct
        );
    }

    /// <summary>
    /// Use the given (targetable) skill on specified entity.
    /// </summary>
    /// <remarks>
    /// The skill won't be used if it is on cooldown.
    /// For skills that can be used only on self, use <paramref name="entityId"/> of the character.
    /// For skills that cannot be targeted on an entity, proceed to <see cref="UseSkillAt"/>.
    /// </remarks>
    /// <param name="skill">The skill to use.</param>
    /// <param name="entity">The entity to use the skill on.</param>
    /// <param name="mapX">The x coordinate on the map. (Used for non targeted dashes etc., says where the dash will be to.)</param>
    /// <param name="mapY">The y coordinate on the map. (Used for non targeted dashes etc., says where the dash will be to.)</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> UseSkillOn
    (
        Skill skill,
        ILivingEntity entity,
        short? mapX = default,
        short? mapY = default,
        CancellationToken ct = default
    )
    {
        if (skill.IsOnCooldown)
        {
            return Task.FromResult<Result>(new SkillOnCooldownError(skill));
        }

        return _client.SendPacketAsync
        (
            new UseSkillPacket
            (
                skill.SkillVNum,
                entity.Type,
                entity.Id,
                mapX,
                mapY
            ),
            ct
        );
    }

    /// <summary>
    /// Use the given (targetable) skill on specified entity.
    /// </summary>
    /// <remarks>
    /// For skills that can be used only on self, use <paramref name="entityId"/> of the character.
    /// For skills that cannot be targeted on an entity, proceed to <see cref="UseSkillAt"/>.
    /// </remarks>
    /// <param name="skill">The skill to use.</param>
    /// <param name="entityId">The id of the entity to use the skill on.</param>
    /// <param name="entityType">The type of the supplied entity.</param>
    /// <param name="mapX">The x coordinate on the map. (Used for non targeted dashes etc., says where the dash will be to.)</param>
    /// <param name="mapY">The y coordinate on the map. (Used for non targeted dashes etc., says where the dash will be to.)</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> UseSkillOn
    (
        Skill skill,
        long entityId,
        EntityType entityType,
        short? mapX = default,
        short? mapY = default,
        CancellationToken ct = default
    )
    {
        if (skill.IsOnCooldown)
        {
            return Task.FromResult<Result>(new SkillOnCooldownError(skill));
        }

        return _client.SendPacketAsync
        (
            new UseSkillPacket
            (
                skill.SkillVNum,
                entityType,
                entityId,
                mapX,
                mapY
            ),
            ct
        );
    }

    /// <summary>
    /// Use the given (aoe) skill on the specified place.
    /// </summary>
    /// <remarks>
    /// For skills that can have targets, proceed to <see cref="UseSkillOn"/>.
    /// </remarks>
    /// <param name="skillVNum">The id of the skill.</param>
    /// <param name="mapX">The x coordinate to use the skill at.</param>
    /// <param name="mapY">The y coordinate to use the skill at.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> UseSkillAt
    (
        long skillVNum,
        short mapX,
        short mapY,
        CancellationToken ct = default
    )
    {
        return _client.SendPacketAsync
        (
            new UseAOESkillPacket(skillVNum, mapX, mapY),
            ct
        );
    }

    /// <summary>
    /// Use the given (aoe) skill on the specified place.
    /// </summary>
    /// <remarks>
    /// For skills that can have targets, proceed to <see cref="UseSkillOn"/>.
    /// </remarks>
    /// <param name="skill">The skill to use.</param>
    /// <param name="mapX">The x coordinate to use the skill at.</param>
    /// <param name="mapY">The y coordinate to use the skill at.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> UseSkillAt
    (
        Skill skill,
        short mapX,
        short mapY,
        CancellationToken ct = default
    )
    {
        if (skill.IsOnCooldown)
        {
            return Task.FromResult<Result>(new SkillOnCooldownError(skill));
        }

        return _client.SendPacketAsync
        (
            new UseAOESkillPacket(skill.SkillVNum, mapX, mapY),
            ct
        );
    }
}