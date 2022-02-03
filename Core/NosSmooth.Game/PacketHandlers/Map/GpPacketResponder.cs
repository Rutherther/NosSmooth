//
//  GpPacketResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Info;
using NosSmooth.Game.Data.Maps;
using NosSmooth.Packets.Server.Portals;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Map;

/// <summary>
/// Responder to gp packet.
/// </summary>
public class GpPacketResponder : IPacketResponder<GpPacket>
{
    private readonly Game _game;

    /// <summary>
    /// Initializes a new instance of the <see cref="GpPacketResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    public GpPacketResponder(Game game)
    {
        _game = game;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<GpPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        await _game.CreateOrUpdateMapAsync
        (
            () => null,
            (map) => map! with
            {
                Portals = map.Portals.Concat
                (
                    new[]
                    {
                        new Portal
                        (
                            packet.PortalId,
                            new Position(packet.X, packet.Y),
                            packet.TargetMapId,
                            packet.PortalType,
                            packet.IsDisabled
                        )
                    }
                ).ToArray()
            },
            ct: ct
        );

        return Result.FromSuccess();
    }
}