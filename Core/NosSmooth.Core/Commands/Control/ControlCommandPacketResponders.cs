//
//  ControlCommandPacketResponders.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using NosSmooth.Core.Commands.Walking;
using NosSmooth.Core.Packets;
using NosSmooth.Packets.Client.Movement;
using NosSmooth.Packets.Server.Maps;
using Remora.Results;

namespace NosSmooth.Core.Commands.Control;

/// <summary>
/// Packet responder for cancellation of <see cref="TakeControlCommand"/>.
/// </summary>
public class ControlCommandPacketResponders : IPacketResponder<CMapPacket>
{
    private readonly ControlCommands _controlCommands;

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlCommandPacketResponders"/> class.
    /// </summary>
    /// <param name="controlCommands">The control commands.</param>
    public ControlCommandPacketResponders(ControlCommands controlCommands)
    {
        _controlCommands = controlCommands;
    }

    /// <inheritdoc />
    public Task<Result> Respond(PacketEventArgs<CMapPacket> packet, CancellationToken ct = default)
        => _controlCommands.CancelAsync(ControlCommandsFilter.MapChangeCancellable, ct: ct);
}