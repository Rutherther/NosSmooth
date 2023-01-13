//
//  UnsafeSkillsApi.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Client;
using NosSmooth.Core.Contracts;
using NosSmooth.Game.Contracts;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Errors;
using NosSmooth.Game.Events.Battle;
using NosSmooth.Packets.Client.Battle;
using NosSmooth.Packets.Enums.Entities;
using NosSmooth.Packets.Server.Skills;
using Remora.Results;

namespace NosSmooth.Game.Apis.Unsafe;

/// <summary>
/// Packet api for using character skills.
/// </summary>
public class UnsafeSkillsApi
{
    private readonly INostaleClient _client;
    private readonly Game _game;
    private readonly Contractor _contractor;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsafeSkillsApi"/> class.
    /// </summary>
    /// <param name="client">The nostale client.</param>
    /// <param name="game">The game.</param>
    /// <param name="contractor">The contractor.</param>
    public UnsafeSkillsApi(INostaleClient client, Game game, Contractor contractor)
    {
        _client = client;
        _game = game;
        _contractor = contractor;
    }

    /// <summary>
    /// Use the given (targetable) skill on specified entity.
    /// </summary>
    /// <remarks>
    /// For skills that can be used only on self, use <paramref name="entityId"/> of the character.
    /// For skills that cannot be targeted on an entity, proceed to UseSkillAt.
    /// </remarks>
    /// <param name="castId">The cast id of the skill.</param>
    /// <param name="entityId">The id of the entity to use the skill on.</param>
    /// <param name="entityType">The type of the supplied entity.</param>
    /// <param name="mapX">The x coordinate on the map. (Used for non targeted dashes etc., says where the dash will be to.)</param>
    /// <param name="mapY">The y coordinate on the map. (Used for non targeted dashes etc., says where the dash will be to.)</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> UseSkillOn
    (
        short castId,
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
                castId,
                entityType,
                entityId,
                mapX,
                mapY
            ),
            ct
        );
    }

    /// <summary>
    /// Use the given (targetable) skill on character itself.
    /// </summary>
    /// <remarks>
    /// For skills that cannot be targeted on an entity, proceed to UseSkillAt.
    /// </remarks>
    /// <param name="castId">The cast id of the skill.</param>
    /// <param name="mapX">The x coordinate on the map. (Used for non targeted dashes etc., says where the dash will be to.)</param>
    /// <param name="mapY">The y coordinate on the map. (Used for non targeted dashes etc., says where the dash will be to.)</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public async Task<Result> UseSkillOnCharacter
    (
        short castId,
        short? mapX = default,
        short? mapY = default,
        CancellationToken ct = default
    )
    {
        var character = _game.Character;
        if (character is null)
        {
            return new NotInitializedError("Character");
        }

        return await _client.SendPacketAsync
        (
            new UseSkillPacket
            (
                castId,
                EntityType.Player,
                character.Id,
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
    /// For skills that can be used only on self, use <paramref name="entity"/> of the character.
    /// For skills that cannot be targeted on an entity, proceed to UseSkillAt.
    /// </remarks>
    /// <param name="castId">The cast id of the skill.</param>
    /// <param name="entity">The entity to use the skill on.</param>
    /// <param name="mapX">The x coordinate on the map. (Used for non targeted dashes etc., says where the dash will be to.)</param>
    /// <param name="mapY">The y coordinate on the map. (Used for non targeted dashes etc., says where the dash will be to.)</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> UseSkillOn
    (
        short castId,
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
                castId,
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
    /// For skills that can be used only on self, use <paramref name="entity"/> of the character.
    /// For skills that cannot be targeted on an entity, proceed to UseSkillAt.
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
        if (skill.Info is null)
        {
            return Task.FromResult<Result>(new NotInitializedError("skill info"));
        }

        return _client.SendPacketAsync
        (
            new UseSkillPacket
            (
                skill.Info.CastId,
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
    /// For skills that cannot be targeted on an entity, proceed to UseSkillAt.
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
        var info = skill.Info;
        if (info is null)
        {
            return Task.FromResult<Result>(new NotInitializedError("skill info"));
        }

        return _client.SendPacketAsync
        (
            new UseSkillPacket
            (
                info.CastId,
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
    /// For skills that can have targets, proceed to UseSkillOn.
    /// </remarks>
    /// <param name="castId">The id of the skill.</param>
    /// <param name="mapX">The x coordinate to use the skill at.</param>
    /// <param name="mapY">The y coordinate to use the skill at.</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> UseSkillAt
    (
        long castId,
        short mapX,
        short mapY,
        CancellationToken ct = default
    )
    {
        return _client.SendPacketAsync
        (
            new UseAOESkillPacket(castId, mapX, mapY),
            ct
        );
    }

    /// <summary>
    /// Use the given (aoe) skill on the specified place.
    /// </summary>
    /// <remarks>
    /// For skills that can have targets, proceed to UseSkillOn.
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
        var info = skill.Info;
        if (info is null)
        {
            return Task.FromResult<Result>(new NotInitializedError("skill info"));
        }

        return _client.SendPacketAsync
        (
            new UseAOESkillPacket(info.CastId, mapX, mapY),
            ct
        );
    }

    /// <summary>
    /// Creates a contract for using a skill on the given entity.
    /// </summary>
    /// <param name="skill">The skill to use.</param>
    /// <param name="entityId">The id of the entity to use the skill on.</param>
    /// <param name="entityType">The type of the supplied entity.</param>
    /// <param name="mapX">The x coordinate on the map. (Used for non targeted dashes etc., says where the dash will be to.)</param>
    /// <param name="mapY">The y coordinate on the map. (Used for non targeted dashes etc., says where the dash will be to.)</param>
    /// <returns>The contract or an error.</returns>
    public Result<IContract<SkillUsedEvent, UseSkillStates>> ContractUseSkillOn
    (
        Skill skill,
        long entityId,
        EntityType entityType,
        short? mapX = default,
        short? mapY = default
    )
    {
        var characterId = _game?.Character?.Id;
        if (characterId is null)
        {
            return new NotInitializedError("Game.Character");
        }

        if (skill.Info is null)
        {
            return new NotInitializedError("skill info");
        }

        return Result<IContract<SkillUsedEvent, UseSkillStates>>.FromSuccess
        (
            CreateUseSkillContract
            (
                _contractor,
                skill.SkillVNum,
                characterId.Value,
                ct => UseSkillOn
                (
                    skill.Info.CastId,
                    entityId,
                    entityType,
                    mapX,
                    mapY,
                    ct
                )
            )
        );
    }

    /// <summary>
    /// Creates a contract for using a skill at the given location.
    /// </summary>
    /// <param name="skill">The skill to use.</param>
    /// <param name="mapX">The x coordinate to use the skill at.</param>
    /// <param name="mapY">The y coordinate to use the skill at.</param>
    /// <returns>The contract or an error.</returns>
    public Result<IContract<SkillUsedEvent, UseSkillStates>> ContractUseSkillAt
    (
        Skill skill,
        short mapX,
        short mapY
    )
    {
        var characterId = _game?.Character?.Id;
        if (characterId is null)
        {
            return new NotInitializedError("Game.Character");
        }

        if (skill.Info is null)
        {
            return new NotInitializedError("skill info");
        }

        return Result<IContract<SkillUsedEvent, UseSkillStates>>.FromSuccess
        (
            CreateUseSkillContract
            (
                _contractor,
                skill.SkillVNum,
                characterId.Value,
                ct =>
                {
                    return UseSkillAt(skill.Info.CastId, mapX, mapY, ct);
                }
            )
        );
    }

    /// <summary>
    /// Creates a use skill contract,
    /// casting the skill using the given action.
    /// </summary>
    /// <param name="contractor">The contractor to register the contract at.</param>
    /// <param name="skillVNum">The vnum of the casting skill.</param>
    /// <param name="characterId">The id of the caster, character.</param>
    /// <param name="useSkill">The used skill event.</param>
    /// <returns>A contract for using the given skill.</returns>
    public static IContract<SkillUsedEvent, UseSkillStates> CreateUseSkillContract
    (
        Contractor contractor,
        int skillVNum,
        long characterId,
        Func<CancellationToken, Task<Result>> useSkill
    )
    {
        return new ContractBuilder<SkillUsedEvent, UseSkillStates, UseSkillErrors>(contractor, UseSkillStates.None)
            .SetMoveAction
            (
                UseSkillStates.None,
                async (data, ct) => (await useSkill(ct)).Map(true),
                UseSkillStates.SkillUseRequested
            )
            .SetMoveFilter<SkillUsedEvent>
            (
                UseSkillStates.SkillUseRequested,
                data => data.Skill.SkillVNum == skillVNum && data.Caster.Id == characterId,
                UseSkillStates.SkillUsedResponse
            )
            .SetFillData<SkillUsedEvent>
            (
                UseSkillStates.SkillUsedResponse,
                skillUseEvent => skillUseEvent
            )
            .SetError<CancelPacket>(UseSkillStates.SkillUseRequested, _ => UseSkillErrors.Unknown)
            .SetTimeout(UseSkillStates.SkillUsedResponse, TimeSpan.FromSeconds(1), UseSkillStates.CharacterRestored)
            .Build();
    }
}