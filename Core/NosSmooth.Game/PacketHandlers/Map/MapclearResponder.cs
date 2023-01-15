//
//  MapclearResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Packets.Server.Maps;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Map;

/// <summary>
/// A responder to <see cref="MapclearPacket"/>.
/// </summary>
public class MapclearResponder : IPacketResponder<MapclearPacket>
{
    private readonly Game _game;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapclearResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    public MapclearResponder(Game game)
    {
        _game = game;
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<MapclearPacket> packetArgs, CancellationToken ct = default)
    {
        _game.CurrentMap?.Entities.Clear();
        return Task.FromResult(Result.FromSuccess());
    }
}