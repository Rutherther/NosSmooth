//
//  InPacketConverter.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.Packets.Packets.Server.Entities;
using NosSmooth.Packets.Packets.Server.Map;
using Remora.Results;

namespace NosSmooth.Packets.Converters.Packets;

/// <summary>
/// Converter for "in" packet. <see cref="InPacket"/>.
/// </summary>
public class InPacketConverter : BaseTypeConverter<InPacket>
{
    /// <inheritdoc />
    public override Result Serialize(InPacket? obj, PacketStringBuilder builder)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public override Result<InPacket?> Deserialize(PacketStringEnumerator stringEnumerator)
    {
        throw new System.NotImplementedException();
    }
}