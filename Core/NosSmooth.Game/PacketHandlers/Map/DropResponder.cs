//
//  DropResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using NosSmooth.Core.Extensions;
using NosSmooth.Core.Packets;
using NosSmooth.Data.Abstractions;
using NosSmooth.Game.Data.Entities;
using NosSmooth.Game.Data.Info;
using NosSmooth.Game.Events.Core;
using NosSmooth.Game.Events.Entities;
using NosSmooth.Packets.Server.Entities;
using NosSmooth.Packets.Server.Maps;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Map;

/// <summary>
/// Responds to drop packet.
/// </summary>
public class DropResponder : IPacketResponder<DropPacket>
{
    private readonly Game _game;
    private readonly EventDispatcher _eventDispatcher;
    private readonly IInfoService _infoService;
    private readonly ILogger<DropResponder> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DropResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    /// <param name="eventDispatcher">The event dispatcher.</param>
    /// <param name="infoService">The info service.</param>
    /// <param name="logger">The logger.</param>
    public DropResponder
    (
        Game game,
        EventDispatcher eventDispatcher,
        IInfoService infoService,
        ILogger<DropResponder> logger
    )
    {
        _game = game;
        _eventDispatcher = eventDispatcher;
        _infoService = infoService;
        _logger = logger;

    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<DropPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var itemInfoResult = await _infoService.GetItemInfoAsync(packet.ItemVNum, ct);
        if (!itemInfoResult.IsDefined(out var itemInfo))
        {
            _logger.LogWarning("Could not obtain item info for vnum {vnum}: {error}", packet.ItemVNum, itemInfoResult.ToFullString());
        }

        var entity = new GroundItem
        {
            Amount = packet.Amount,
            Id = packet.DropId,
            IsQuestRelated = packet.IsQuestRelated,
            ItemInfo = itemInfo,
            OwnerId = null,
            Position = new Position
            (
                packet.X,
                packet.Y
            ),
            VNum = packet.ItemVNum
        };

        var map = _game.CurrentMap;
        if (map is not null)
        {
            map.Entities.AddEntity(entity);
        }

        return await _eventDispatcher.DispatchEvent(new ItemDroppedEvent(entity), ct);
    }
}