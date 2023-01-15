//
//  RaidHpMpResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Info;
using NosSmooth.Game.Data.Social;
using NosSmooth.Packets.Server.Raids;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Raids;

/// <summary>
/// A responder to <see cref="RaidfhpPacket"/>, <see cref="RaidfmpPacket"/>.
/// </summary>
public class RaidHpMpResponder : IPacketResponder<RaidfhpPacket>, IPacketResponder<RaidfmpPacket>
{
    private readonly Game _game;

    /// <summary>
    /// Initializes a new instance of the <see cref="RaidHpMpResponder"/> class.
    /// </summary>
    /// <param name="game">The game.</param>
    public RaidHpMpResponder(Game game)
    {
        _game = game;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<RaidfhpPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;

        await _game.UpdateRaidAsync
        (
            raid =>
            {
                foreach (var member in raid.Members ?? (IReadOnlyList<GroupMember>)Array.Empty<GroupMember>())
                {
                    var data = packet.HpSubPackets?.FirstOrDefault(x => x.PlayerId == member.PlayerId);

                    if (data is not null)
                    {
                        member.Hp ??= new Health();
                        member.Hp.Percentage = data.HpPercentage;
                    }
                }
                return raid;
            },
            ct: ct
        );
        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<RaidfmpPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;

        await _game.UpdateRaidAsync
        (
            raid =>
            {
                foreach (var member in raid.Members ?? (IReadOnlyList<GroupMember>)Array.Empty<GroupMember>())
                {
                    var data = packet.HpSubPackets?.FirstOrDefault(x => x.PlayerId == member.PlayerId);

                    if (data is not null)
                    {
                        member.Mp ??= new Health();
                        member.Mp.Percentage = data.HpPercentage;
                    }
                }
                return raid;
            },
            ct: ct
        );
        return Result.FromSuccess();
    }
}