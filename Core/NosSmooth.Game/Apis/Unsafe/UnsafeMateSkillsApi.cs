//
//  UnsafeMateSkillsApi.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Client;
using NosSmooth.Packets.Client.Battle;
using NosSmooth.Packets.Enums.Entities;
using Remora.Results;

namespace NosSmooth.Game.Apis.Unsafe;

/// <summary>
/// Packet api for using mate skills.
/// </summary>
public class UnsafeMateSkillsApi
{
    private readonly ManagedNostaleClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsafeMateSkillsApi"/> class.
    /// </summary>
    /// <param name="client">The client.</param>
    public UnsafeMateSkillsApi(ManagedNostaleClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Use a pet skill.
    /// </summary>
    /// <param name="petId">The pet id.</param>
    /// <param name="targetEntityType">The type of the target entity.</param>
    /// <param name="targetId">The id of the target.</param>
    /// <param name="mapX">The x coordinate of the partner.</param>
    /// <param name="mapY">The y coordinate of the partner.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> UsePetSkillAsync
    (
        long petId,
        EntityType targetEntityType,
        long targetId,
        short? mapX = default,
        short? mapY = default
    )
    {
        return _client.SendPacketAsync
        (
            new UsePetSkillPacket
            (
                petId,
                targetEntityType,
                targetId,
                1,
                mapX,
                mapY
            )
        );
    }

    /// <summary>
    /// Use a partner skill.
    /// </summary>
    /// <param name="partnerId">The pet id.</param>
    /// <param name="skillSlot">The slot of the skill.</param>
    /// <param name="targetEntityType">The type of the target entity.</param>
    /// <param name="targetId">The id of the target.</param>
    /// <param name="mapX">The x coordinate of the partner.</param>
    /// <param name="mapY">The y coordinate of the partner.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> UsePartnerSkillAsync
    (
        long partnerId,
        byte skillSlot,
        EntityType targetEntityType,
        long targetId,
        short? mapX = default,
        short? mapY = default
    )
    {
        return _client.SendPacketAsync
        (
            new UsePartnerSkillPacket
            (
                partnerId,
                targetEntityType,
                targetId,
                skillSlot,
                mapX,
                mapY
            )
        );
    }
}