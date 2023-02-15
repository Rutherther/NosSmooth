//
//  CMapResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Data.Abstractions;
using NosSmooth.Packets.Server.Maps;
using Remora.Results;

namespace NosSmooth.Extensions.Pathfinding.Responders;

/// <inheritdoc />
internal class CMapResponder : IPacketResponder<CMapPacket>
{
    private readonly PathfinderState _state;
    private readonly IInfoService _infoService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CMapResponder"/> class.
    /// </summary>
    /// <param name="state">The state.</param>
    /// <param name="infoService">The info service.</param>
    public CMapResponder(PathfinderState state, IInfoService infoService)
    {
        _state = state;
        _infoService = infoService;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<CMapPacket> packetArgs, CancellationToken ct = default)
    {
        _state.ClearEntities();
        var packet = packetArgs.Packet;

        _state.MapId = packet.Id;
        var mapInfoResult = await _infoService.GetMapInfoAsync(packet.Id, ct);

        if (!mapInfoResult.IsSuccess)
        {
            return Result.FromError(mapInfoResult);
        }

        _state.MapInfo = mapInfoResult.Entity;
        return Result.FromSuccess();
    }
}