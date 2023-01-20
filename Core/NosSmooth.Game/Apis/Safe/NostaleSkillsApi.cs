//
//  NostaleSkillsApi.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Client;
using NosSmooth.Core.Contracts;
using NosSmooth.Data.Abstractions.Enums;
using NosSmooth.Game.Apis.Unsafe;
using NosSmooth.Game.Contracts;
using NosSmooth.Game.Data.Characters;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Info;
using NosSmooth.Game.Errors;
using NosSmooth.Game.Events.Battle;
using NosSmooth.Packets.Client.Battle;
using Remora.Results;

namespace NosSmooth.Game.Apis.Safe;

/// <summary>
/// A safe NosTale api for using character skills.
/// </summary>
public class NostaleSkillsApi
{
    private readonly Game _game;
    private readonly INostaleClient _client;
    private readonly Contractor _contractor;

    /// <summary>
    /// Initializes a new instance of the <see cref="NostaleSkillsApi"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="client">The NosTale client.</param>
    /// <param name="contractor">The contractor.</param>
    public NostaleSkillsApi(Game game, INostaleClient client, Contractor contractor)
    {
        _game = game;
        _client = client;
        _contractor = contractor;
    }

    /// <summary>
    /// Use the given (targetable) skill on character himself.
    /// </summary>
    /// <param name="skill">The skill to use.</param>
    /// <param name="mapX">The x coordinate on the map. (Used for non targeted dashes etc., says where the dash will be to.)</param>
    /// <param name="mapY">The y coordinate on the map. (Used for non targeted dashes etc., says where the dash will be to.)</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> UseSkillOnCharacter
    (
        Skill skill,
        short? mapX = default,
        short? mapY = default,
        CancellationToken ct = default
    )
    {
        var character = _game.Character;
        if (character is null)
        {
            return Task.FromResult<Result>(new NotInitializedError("Game.Character"));
        }

        var skills = _game.Skills;
        if (skills is null)
        {
            return Task.FromResult<Result>(new NotInitializedError("Game.Skills"));
        }

        if (skill.IsOnCooldown)
        {
            return Task.FromResult<Result>(new SkillOnCooldownError(skill));
        }

        if (skill.Info is null)
        {
            return Task.FromResult<Result>(new NotInitializedError("skill info"));
        }

        if (skill.Info.TargetType is not(TargetType.Self or TargetType.SelfOrTarget))
        {
            return Task.FromResult<Result>(new WrongSkillTargetError(skill, character));
        }

        if (skill.Info.SkillType != SkillType.Player)
        {
            return Task.FromResult<Result>(new WrongSkillTypeError(SkillType.Player, skill.Info.SkillType));
        }

        if (skill.Info.AttackType == AttackType.Dash && (mapX is null || mapY is null))
        {
            return Task.FromResult<Result>(new WrongSkillPositionError(skill.Info.AttackType));
        }
        if (skill.Info.AttackType != AttackType.Dash && (mapX is not null || mapY is not null))
        {
            return Task.FromResult<Result>(new WrongSkillPositionError(skill.Info.AttackType));
        }

        var characterPosition = _game.Character?.Position;
        if (characterPosition is null)
        {
            return Task.FromResult<Result>(new NotInitializedError("character position"));
        }

        if (mapX != null && mapY != null)
        {
            var mapPosition = new Position(mapX.Value, mapY.Value);
            if (!mapPosition.IsInRange(characterPosition.Value, skill.Info.Range))
            {
                return Task.FromResult<Result>
                (
                    new NotInRangeError
                    (
                        "Character",
                        characterPosition.Value,
                        mapPosition,
                        skill.Info.Range
                    )
                );
            }
        }

        return _client.SendPacketAsync
        (
            new UseSkillPacket
            (
                skill.Info.CastId,
                character.Type,
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
    /// The skill won't be used if it is on cooldown.
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
        if (entity == _game.Character)
        {
            return UseSkillOnCharacter(skill, mapX, mapY, ct);
        }

        var skills = _game.Skills;
        if (skills is null)
        {
            return Task.FromResult<Result>(new NotInitializedError("Game.Skills"));
        }

        if (skill.IsOnCooldown)
        {
            return Task.FromResult<Result>(new SkillOnCooldownError(skill));
        }

        if (skill.Info is null)
        {
            return Task.FromResult<Result>(new NotInitializedError("skill info"));
        }

        if (skill.Info.TargetType is not(TargetType.Target or TargetType.SelfOrTarget))
        {
            return Task.FromResult<Result>(new WrongSkillTargetError(skill, entity));
        }

        if (skill.Info.SkillType != SkillType.Player)
        {
            return Task.FromResult<Result>(new WrongSkillTypeError(SkillType.Player, skill.Info.SkillType));
        }

        if (skill.Info.AttackType == AttackType.Dash && (mapX is null || mapY is null))
        {
            return Task.FromResult<Result>(new WrongSkillPositionError(skill.Info.AttackType));
        }
        if (skill.Info.AttackType != AttackType.Dash && (mapX is not null || mapY is not null))
        {
            return Task.FromResult<Result>(new WrongSkillPositionError(skill.Info.AttackType));
        }

        var entityPosition = entity.Position;
        if (entityPosition is null)
        {
            return Task.FromResult<Result>(new NotInitializedError("entity position"));
        }

        var characterPosition = _game.Character?.Position;
        if (characterPosition is null)
        {
            return Task.FromResult<Result>(new NotInitializedError("character position"));
        }

        if (!entityPosition.Value.IsInRange(characterPosition.Value, skill.Info.Range))
        {
            return Task.FromResult<Result>
            (
                new NotInRangeError
                (
                    "Character",
                    characterPosition.Value,
                    entityPosition.Value,
                    skill.Info.Range
                )
            );
        }

        if (mapX != null && mapY != null)
        {
            var mapPosition = new Position(mapX.Value, mapY.Value);
            if (!mapPosition.IsInRange(characterPosition.Value, skill.Info.Range))
            {
                return Task.FromResult<Result>
                (
                    new NotInRangeError
                    (
                        "Character",
                        characterPosition.Value,
                        mapPosition,
                        skill.Info.Range
                    )
                );
            }
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
    /// <param name="mapX">The x coordinate on the map. (Used for non targeted dashes etc., says where the dash will be to.)</param>
    /// <param name="mapY">The y coordinate on the map. (Used for non targeted dashes etc., says where the dash will be to.)</param>
    /// <param name="ct">The cancellation token for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> UseSkillOn
    (
        Skill skill,
        long entityId,
        short? mapX = default,
        short? mapY = default,
        CancellationToken ct = default
    )
    {
        var map = _game.CurrentMap;
        if (map is null)
        {
            return Task.FromResult<Result>(new NotInitializedError("Game.Map"));
        }

        var entity = map.Entities.GetEntity<ILivingEntity>(entityId);
        if (entity is null)
        {
            return Task.FromResult<Result>(new NotFoundError($"Entity with id {entityId} was not found on the map."));
        }

        return UseSkillOn
        (
            skill,
            entity,
            mapX,
            mapY,
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
        var skills = _game.Skills;
        if (skills is null)
        {
            return Task.FromResult<Result>(new NotInitializedError("Game.Skills"));
        }

        if (skill.IsOnCooldown)
        {
            return Task.FromResult<Result>(new SkillOnCooldownError(skill));
        }

        if (skill.Info is null)
        {
            return Task.FromResult<Result>(new NotInitializedError("skill info"));
        }

        if (skill.Info.TargetType is not TargetType.NoTarget)
        {
            return Task.FromResult<Result>(new WrongSkillTargetError(skill, null));
        }

        if (skill.Info.SkillType != SkillType.Player)
        {
            return Task.FromResult<Result>(new WrongSkillTypeError(SkillType.Player, skill.Info.SkillType));
        }

        var characterPosition = _game.Character?.Position;
        if (characterPosition is null)
        {
            return Task.FromResult<Result>(new NotInitializedError("character position"));
        }

        var target = new Position(mapX, mapY);
        if (!target.IsInRange(characterPosition.Value, skill.Info.Range))
        {
            return Task.FromResult<Result>
            (
                new NotInRangeError
                (
                    "Character",
                    characterPosition.Value,
                    target,
                    skill.Info.Range
                )
            );
        }

        return _client.SendPacketAsync
        (
            new UseAOESkillPacket(skill.Info.CastId, mapX, mapY),
            ct
        );
    }

    /// <summary>
    /// Creates a contract for using a skill on character himself.
    /// </summary>
    /// <param name="skill">The skill to use.</param>
    /// <param name="mapX">The x coordinate to use the skill at.</param>
    /// <param name="mapY">The y coordinate to use the skill at.</param>
    /// <returns>The contract or an error.</returns>
    public Result<IContract<SkillUsedEvent, UseSkillStates>> ContractUseSkillOnCharacter
    (
        Skill skill,
        short? mapX = default,
        short? mapY = default
    )
    {
        var characterId = _game?.Character?.Id;
        if (characterId is null)
        {
            return new NotInitializedError("Game.Character");
        }

        return Result<IContract<SkillUsedEvent, UseSkillStates>>.FromSuccess
        (
            UnsafeSkillsApi.CreateUseSkillContract
            (
                _contractor,
                skill.SkillVNum,
                characterId.Value,
                ct => UseSkillOnCharacter
                (
                    skill,
                    mapX,
                    mapY,
                    ct
                )
            )
        );
    }

    /// <summary>
    /// Creates a contract for using a skill on the given entity.
    /// </summary>
    /// <param name="skill">The skill to use.</param>
    /// <param name="entity">The entity to use the skill on.</param>
    /// <param name="mapX">The x coordinate to use the skill at.</param>
    /// <param name="mapY">The y coordinate to use the skill at.</param>
    /// <returns>The contract or an error.</returns>
    public Result<IContract<SkillUsedEvent, UseSkillStates>> ContractUseSkillOn
    (
        Skill skill,
        ILivingEntity entity,
        short? mapX = default,
        short? mapY = default
    )
    {
        var characterId = _game?.Character?.Id;
        if (characterId is null)
        {
            return new NotInitializedError("Game.Character");
        }

        return Result<IContract<SkillUsedEvent, UseSkillStates>>.FromSuccess
        (
            UnsafeSkillsApi.CreateUseSkillContract
            (
                _contractor,
                skill.SkillVNum,
                characterId.Value,
                ct => UseSkillOn
                (
                    skill,
                    entity,
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

        return Result<IContract<SkillUsedEvent, UseSkillStates>>.FromSuccess
        (
            UnsafeSkillsApi.CreateUseSkillContract
            (
                _contractor,
                skill.SkillVNum,
                characterId.Value,
                ct => UseSkillAt
                (
                    skill,
                    mapX,
                    mapY,
                    ct
                )
            )
        );
    }
}