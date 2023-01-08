//
//  NostaleMatePacketApi.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Client;
using NosSmooth.Game.Attributes;
using NosSmooth.Packets.Client;
using NosSmooth.Packets.Enums.Entities;
using NosSmooth.Packets.Enums.NRun;
using Remora.Results;

namespace NosSmooth.Game.Apis;

/// <summary>
/// Packet api for managing mates, company, stay, sending them back.
/// </summary>
public class NostaleMatePacketApi
{
    private readonly INostaleClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="NostaleMatePacketApi"/> class.
    /// </summary>
    /// <param name="client">The client.</param>
    public NostaleMatePacketApi(INostaleClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Make the given pet your company (possible only in miniland).
    /// </summary>
    /// <remarks>
    /// May be used only in miniland.
    ///
    /// Unsafe, does not check whether the mate exists
    /// or whether the character is in miniland.
    /// </remarks>
    /// <param name="mateId">The id of the mate to have company.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> CompanyAsync(long mateId, CancellationToken ct = default)
        => _client.SendPacketAsync
        (
            new NRunPacket
            (
                NRunType.Mate,
                (short)MateNRunType.Company,
                EntityType.Npc,
                mateId,
                null
            ),
            ct
        );

    /// <summary>
    /// Make the given pet stay in your miniland (possible only in miniland).
    /// </summary>
    /// <remarks>
    /// May be used only in miniland.
    ///
    /// Unsafe, does not check whether the mate exists
    /// or whether the character is in miniland.
    /// </remarks>
    /// <param name="mateId">The id of the mate to have company.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> StayAsync(long mateId, CancellationToken ct = default)
        => _client.SendPacketAsync
        (
            new NRunPacket
            (
                NRunType.Mate,
                (short)MateNRunType.Stay,
                EntityType.Npc,
                mateId,
                null
            ),
            ct
        );

    /// <summary>
    /// Save the given pet back to miniland.
    /// </summary>
    /// <remarks>
    /// Unsafe, does not check whether the mate exists.
    /// </remarks>
    /// <param name="mateId">The id of the mate to have company.</param>
    /// <param name="ct">The cancellation token used for cancelling the operation.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Task<Result> SendBackAsync(long mateId, CancellationToken ct = default)
        => _client.SendPacketAsync
        (
            new NRunPacket
            (
                NRunType.Mate,
                (short)MateNRunType.RequestPetSendBack,
                EntityType.Npc,
                mateId,
                null
            ),
            ct
        );
}