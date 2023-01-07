//
//  FriendInitResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Chat;
using NosSmooth.Packets.Enums.Relations;
using NosSmooth.Packets.Server.Relations;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Relations;

/// <summary>
/// A friends initialization responder.
/// </summary>
public class FriendInitResponder : IPacketResponder<FInfoPacket>, IPacketResponder<FInitPacket>
{
    private readonly Game _game;

    /// <summary>
    /// Initializes a new instance of the <see cref="FriendInitResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    public FriendInitResponder(Game game)
    {
        _game = game;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<FInfoPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;

        await _game.CreateOrUpdateFriendsAsync
        (
            () => null,
            friends => friends
                .Select(
                    x =>
                    {
                        var subPacket = packet.FriendSubPackets.FirstOrDefault(y => x.PlayerId == y.PlayerId);
                        if (subPacket is not null)
                        {
                            x.IsConnected = subPacket.IsConnected;
                            x.CharacterName = subPacket.Name;
                        }

                        return x;
                    })
                .ToList(),
            ct: ct
        );

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<FInitPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        var friends = packet.FriendSubPackets
            .Select
            (
                x => new Friend(x.PlayerId, x.RelationType)
                {
                    PlayerId = x.PlayerId,
                    CharacterName = x.Name,
                    IsConnected = x.IsConnected
                }
            )
            .ToList();

        await _game.CreateOrUpdateFriendsAsync
        (
            () => friends,
            _ => friends,
            ct: ct
        );

        return Result.FromSuccess();
    }
}