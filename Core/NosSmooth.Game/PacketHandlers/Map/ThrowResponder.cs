//
//  ThrowResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using NosSmooth.Core.Extensions;
using NosSmooth.Core.Packets;
using NosSmooth.Data.Abstractions;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Info;
using NosSmooth.Packets.Server.Maps;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Map;

/// <summary>
/// A responder to <see cref="ThrowResponder"/>.
/// </summary>
public class ThrowResponder : IPacketResponder<ThrowPacket>
{
    private readonly Game _game;
    private readonly IInfoService _infoService;
    private readonly ILogger<ThrowResponder> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThrowResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="infoService">The info service.</param>
    /// <param name="logger">The logger.</param>
    public ThrowResponder(Game game, IInfoService infoService, ILogger<ThrowResponder> logger)
    {
        _game = game;
        _infoService = infoService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<ThrowPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var map = _game.CurrentMap;

        if (map is null)
        {
            return Result.FromSuccess();
        }

        var itemInfoResult = await _infoService.GetItemInfoAsync(packet.ItemVNum, ct);
        if (!itemInfoResult.IsDefined(out var itemInfo))
        {
            _logger.LogWarning("Could not obtain item info for vnum {vnum}: {error}", packet.ItemVNum, itemInfoResult.ToFullString());
        }

        map.Entities.AddEntity
        (
            new GroundItem
            {
                Position = new Position(packet.TargetX, packet.TargetY),
                Amount = packet.Amount,
                Id = packet.DropId,
                ItemInfo = itemInfo,
                VNum = packet.ItemVNum
            }
        );

        return Result.FromSuccess();
    }
}