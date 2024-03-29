//
//  CMapResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using NosSmooth.Core.Extensions;
using NosSmooth.Core.Packets;
using NosSmooth.Data.Abstractions;
using NosSmooth.Game.Data.Maps;
using NosSmooth.Game.Events.Characters;
using NosSmooth.Game.Events.Core;
using NosSmooth.Game.Events.Map;
using NosSmooth.Packets.Server.Maps;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Map;

/// <summary>
/// Responds to <see cref="CMapResponder"/> by creating a new map.
/// </summary>
public class CMapResponder : IPacketResponder<CMapPacket>
{
    private readonly Game _game;
    private readonly EventDispatcher _eventDispatcher;
    private readonly IInfoService _infoService;
    private readonly ILogger<CMapResponder> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CMapResponder"/> class.
    /// </summary>
    /// <param name="game">The nostale game.</param>
    /// <param name="eventDispatcher">The event dispatcher.</param>
    /// <param name="infoService">The info service.</param>
    /// <param name="logger">The logger.</param>
    public CMapResponder
    (
        Game game,
        EventDispatcher eventDispatcher,
        IInfoService infoService,
        ILogger<CMapResponder> logger
    )
    {
        _game = game;
        _eventDispatcher = eventDispatcher;
        _infoService = infoService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<CMapPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var mapInfoResult = await _infoService.GetMapInfoAsync(packet.Id, ct);
        if (!mapInfoResult.IsSuccess)
        {
            _logger.LogWarning
            (
                "Could not obtain a map info for id {id}: {error}",
                packet.Id,
                mapInfoResult.ToFullString()
            );
        }

        var previousMap = _game.CurrentMap;
        var currentMap = await _game.CreateMapAsync
        (
            () =>
            {
                var map = packet.Id == 20001
                    ? new Miniland
                    (
                        packet.Id,
                        packet.Type,
                        mapInfoResult.IsSuccess ? mapInfoResult.Entity : null,
                        new MapEntities(),
                        Array.Empty<Portal>(),
                        Array.Empty<MinilandObject>()
                    )
                    : new Data.Maps.Map
                    (
                        packet.Id,
                        packet.Type,
                        mapInfoResult.IsSuccess ? mapInfoResult.Entity : null,
                        new MapEntities(),
                        Array.Empty<Portal>()
                    );

                var character = _game.Character;
                if (character is not null)
                {
                    map.Entities.AddEntity(character);
                }
                return map;
            },
            ct: ct
        );

        if (currentMap is not null)
        {
            return await _eventDispatcher.DispatchEvent(new MapChangedEvent(previousMap, currentMap), ct);
        }

        return Result.FromSuccess();
    }
}