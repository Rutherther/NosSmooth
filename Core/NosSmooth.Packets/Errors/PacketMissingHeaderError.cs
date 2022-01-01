//
//  PacketMissingHeaderError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Packets;
using Remora.Results;

namespace NosSmooth.Packets.Errors;

/// <summary>
/// Packet is missing header and thus cannot be serialized correctly.
/// </summary>
/// <param name="Packet">The packet that is missing header.</param>
public record PacketMissingHeaderError(IPacket Packet)
    : ResultError($"The packet {Packet.GetType().FullName} is missing a header and cannot be serialized.");