//
//  WalkPacketResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security;
using NosCore.Packets.ClientPackets.Battle;
using NosCore.Packets.ClientPackets.Movement;
using NosCore.Packets.ServerPackets.MiniMap;
using NosSmooth.Core.Commands;
using NosSmooth.Core.Packets;
using NosSmooth.Packets.Packets.Client.Movement;
using NosSmooth.Packets.Packets.Server.Maps;
using Remora.Results;

namespace NosSmooth.LocalClient.CommandHandlers.Walk;

/// <summary>
/// Responds to <see cref="WalkPacket"/> to manage <see cref="WalkCommand"/>.
/// </summary>
public class WalkPacketResponder : IPacketResponder<WalkPacket>, IPacketResponder<CMapPacket>
{
    private readonly WalkStatus _walkStatus;

    /// <summary>
    /// Initializes a new instance of the <see cref="WalkPacketResponder"/> class.
    /// </summary>
    /// <param name="walkStatus">The walk status.</param>
    public WalkPacketResponder(WalkStatus walkStatus)
    {
        _walkStatus = walkStatus;
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<WalkPacket> packet, CancellationToken ct = default)
    {
        if (_walkStatus.IsWalking)
        {
            _walkStatus.UpdateWalkTime(packet.Packet.PositionX, packet.Packet.PositionY);
            if (packet.Packet.PositionX == _walkStatus.TargetX && packet.Packet.PositionY == _walkStatus.TargetY)
            {
                await _walkStatus.FinishWalkingAsync(ct);
            }
        }

        return Result.FromSuccess();
    }

    /// <inheritdoc />
    public async Task<Result> Respond(PacketEventArgs<CMapPacket> packet, CancellationToken ct = default)
    {
        if (_walkStatus.IsWalking)
        {
            await _walkStatus.CancelWalkingAsync(WalkCancelReason.MapChanged, ct);
        }

        return Result.FromSuccess();
    }

    // TODO: handle teleport on map
}
