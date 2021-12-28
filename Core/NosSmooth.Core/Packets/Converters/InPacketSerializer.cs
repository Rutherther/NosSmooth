//
//  InPacketSerializer.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using NosCore.Packets.ServerPackets.Visibility;
using NosCore.Shared.Enumerations;
using Remora.Results;

namespace NosSmooth.Core.Packets.Converters;

/// <summary>
/// Deserializes InPacket correctly.
/// </summary>
public class InPacketSerializer : SpecificPacketSerializer<InPacket>
{
    private readonly PacketSerializerProvider _packetSerializerProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="InPacketSerializer"/> class.
    /// </summary>
    /// <param name="packetSerializerProvider">The provider of packet serializer.</param>
    public InPacketSerializer(PacketSerializerProvider packetSerializerProvider)
    {
        _packetSerializerProvider = packetSerializerProvider;
    }

    /// <inheritdoc />
    public override bool Serializer => false;

    /// <inheritdoc />
    public override bool Deserializer => true;

    /// <inheritdoc />
    public override Result<string> Serialize(InPacket packet)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public override Result<InPacket> Deserialize(string packetString)
    {
        try
        {
            var deserializer = _packetSerializerProvider.ServerSerializer.Deserializer;
            var splitted = packetString.Split(new char[] { ' ' }, 9).Skip(1).ToArray();

            if (!Enum.TryParse(splitted[0], out VisualType type))
            {
                return new ArgumentInvalidError(nameof(packetString), "The visual type is incorrect.");
            }

            var startAddress = type == VisualType.Player ? 3 : 2;
            var inPacket = new InPacket
            {
                VisualType = type,
                VNum = type != VisualType.Player ? long.Parse(splitted[1]) : null,
                Name = type == VisualType.Player ? splitted[1] : null,
                VisualId = long.Parse(splitted[startAddress]),
                PositionX = short.Parse(splitted[startAddress + 1]),
                PositionY = short.Parse(splitted[startAddress + 2]),
                Direction = byte.Parse(splitted[startAddress + 3])
            };

            switch (inPacket.VisualType)
            {
                case VisualType.Player:
                    inPacket.InCharacterSubPacket = (InCharacterSubPacket?)deserializer
                        .DeserializeHeaderlessIPacket(typeof(InCharacterSubPacket), splitted[7]);
                    break;
                case VisualType.Object:
                    inPacket.InItemSubPacket = (InItemSubPacket?)deserializer
                        .DeserializeHeaderlessIPacket(typeof(InItemSubPacket), splitted[6] + " " + splitted[7]);
                    break;
                default:
                    inPacket.InNonPlayerSubPacket = (InNonPlayerSubPacket?)deserializer
                        .DeserializeHeaderlessIPacket(typeof(InNonPlayerSubPacket), splitted[6] + " " + splitted[7]);
                    break;
            }

            return inPacket;
        }
        catch (Exception e)
        {
            return e;
        }
    }
}