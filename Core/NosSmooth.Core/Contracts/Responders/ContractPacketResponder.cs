//
//  ContractPacketResponder.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using NosSmooth.Core.Packets;
using NosSmooth.Packets;
using Remora.Results;

namespace NosSmooth.Core.Contracts.Responders;

/// <summary>
/// A responder that calls Contractor update.
/// </summary>
public class ContractPacketResponder : IEveryPacketResponder
{
    private readonly Contractor _contractor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContractPacketResponder"/> class.
    /// </summary>
    /// <param name="contractor">The contractor.</param>
    public ContractPacketResponder(Contractor contractor)
    {
        _contractor = contractor;
    }

    /// <inheritdoc />
    public Task<Result> Respond<TPacket>(PacketEventArgs<TPacket> packetArgs, CancellationToken ct = default)
        where TPacket : IPacket
        => _contractor.Update(packetArgs.Packet, ct);
}