//
//  FcResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Act4;
using NosSmooth.Game.Events.Act4;
using NosSmooth.Game.Events.Core;
using NosSmooth.Packets.Server.Act4;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Act4;

/// <summary>
/// Responds to <see cref="FcPacket"/>.
/// </summary>
public class FcResponder : IPacketResponder<FcPacket>
{
    private readonly EventDispatcher _eventDispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="FcResponder"/> class.
    /// </summary>
    /// <param name="eventDispatcher">The event dispatcher.</param>
    public FcResponder(EventDispatcher eventDispatcher)
    {
        _eventDispatcher = eventDispatcher;
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<FcPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        return _eventDispatcher.DispatchEvent
        (
            new Act4StatusReceivedEvent
            (
                packet.Faction,
                packet.MinutesUntilReset,
                GetStatus(packet.AngelState),
                GetStatus(packet.DemonState)
            ),
            ct
        );
    }

    private Act4FactionStatus GetStatus(FcSubPacket packet)
    {
        return new Act4FactionStatus
        (
            packet.Percentage,
            packet.Mode,
            packet.CurrentTime,
            packet.TotalTime,
            GetRaid(packet)
        );
    }

    private Act4Raid? GetRaid(FcSubPacket packet)
    {
        if (packet.IsBerios)
        {
            return Act4Raid.Berios;
        }
        if (packet.IsCalvina)
        {
            return Act4Raid.Calvina;
        }
        if (packet.IsHatus)
        {
            return Act4Raid.Hatus;
        }
        if (packet.IsMorcos)
        {
            return Act4Raid.Morcos;
        }

        return null;
    }
}