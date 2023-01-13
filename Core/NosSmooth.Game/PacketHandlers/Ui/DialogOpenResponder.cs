//
//  DialogOpenResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Core.Packets;
using NosSmooth.Game.Data.Dialogs;
using NosSmooth.Game.Events.Core;
using NosSmooth.Game.Events.Ui;
using NosSmooth.Packets.Enums;
using NosSmooth.Packets.Server.UI;
using Remora.Results;

namespace NosSmooth.Game.PacketHandlers.Ui;

/// <summary>
/// A responder to dialog events that calls DialogOpen.
/// </summary>
public class DialogOpenResponder : IPacketResponder<QnamlPacket>, IPacketResponder<Qnamli2Packet>,
    IPacketResponder<QnaPacket>, IPacketResponder<DlgPacket>, IPacketResponder<DlgiPacket>
{
    private readonly EventDispatcher _eventDispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogOpenResponder"/> class.
    /// </summary>
    /// <param name="eventDispatcher">The event dispatcher.</param>
    public DialogOpenResponder(EventDispatcher eventDispatcher)
    {
        _eventDispatcher = eventDispatcher;

    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<QnamlPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        if (packet.Type != QnamlType.Dialog)
        {
            return Task.FromResult(Result.FromSuccess());
        }

        return _eventDispatcher.DispatchEvent
        (
            new DialogOpenedEvent
            (
                new Dialog
                (
                    packet.AcceptCommand,
                    null,
                    packet.Message,
                    Array.Empty<string>()
                )
            ),
            ct
        );
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<Qnamli2Packet> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;
        if (packet.Type != QnamlType.Dialog)
        {
            return Task.FromResult(Result.FromSuccess());
        }

        return _eventDispatcher.DispatchEvent
        (
            new DialogOpenedEvent
            (
                new Dialog
                (
                    packet.AcceptCommand,
                    null,
                    packet.MessageConst,
                    packet.Parameters
                )
            ),
            ct
        );
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<QnaPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;

        return _eventDispatcher.DispatchEvent
        (
            new DialogOpenedEvent
            (
                new Dialog
                (
                    packet.AcceptCommand,
                    null,
                    packet.Message,
                    Array.Empty<string>()
                )
            ),
            ct
        );
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<DlgPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;

        return _eventDispatcher.DispatchEvent
        (
            new DialogOpenedEvent
            (
                new Dialog
                (
                    packet.AcceptCommand,
                    packet.DenyCommand,
                    packet.Message,
                    Array.Empty<string>()
                )
            ),
            ct
        );
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<DlgiPacket> packetArgs, CancellationToken ct = default)
    {
        var packet = packetArgs.Packet;

        return _eventDispatcher.DispatchEvent
        (
            new DialogOpenedEvent
            (
                new Dialog
                (
                    packet.AcceptCommand,
                    packet.DenyCommand,
                    packet.MessageConst,
                    packet.Parameters
                )
            ),
            ct
        );
    }
}