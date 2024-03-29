﻿//
//  PacketSerializer.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.Packets;
using NosSmooth.PacketSerializer.Abstractions;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Abstractions.Errors;
using NosSmooth.PacketSerializer.Errors;
using NosSmooth.PacketSerializer.Packets;
using Remora.Results;

namespace NosSmooth.PacketSerializer;

/// <summary>
/// Serializer of packets.
/// </summary>
public class PacketSerializer : IPacketSerializer
{
    private readonly IPacketTypesRepository _packetTypesRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketSerializer"/> class.
    /// </summary>
    /// <param name="packetTypesRepository">The repository of packet types.</param>
    public PacketSerializer(IPacketTypesRepository packetTypesRepository)
    {
        _packetTypesRepository = packetTypesRepository;
    }

    /// <inheritdoc/>
    public Result<string> Serialize(IPacket obj)
    {
        var stringBuilder = new PacketStringBuilder(stackalloc char[500]);
        var infoResult = _packetTypesRepository.FindPacketInfo(obj.GetType());
        if (!infoResult.IsSuccess)
        {
            return Result<string>.FromError(infoResult);
        }

        var info = infoResult.Entity;
        if (info.Header is null)
        {
            return new PacketMissingHeaderError(obj);
        }

        stringBuilder.Append(info.Header);
        var serializeResult = info.PacketConverter.Serialize(obj, ref stringBuilder);
        if (!serializeResult.IsSuccess)
        {
            return Result<string>.FromError(serializeResult);
        }

        var output = stringBuilder.ToString();
        stringBuilder.Dispose();
        return output;
    }

    /// <inheritdoc/>
    public Result<IPacket> Deserialize(ReadOnlySpan<char> packetString, PacketSource preferredSource)
    {
        var packetStringEnumerator = new PacketStringEnumerator(packetString);
        var headerTokenResult = packetStringEnumerator.GetNextToken(out var packetToken);
        if (!headerTokenResult.IsSuccess)
        {
            return Result<IPacket>.FromError(headerTokenResult);
        }

        var packetInfoResult = _packetTypesRepository.FindPacketInfo(packetToken.Token.ToString(), preferredSource);
        if (!packetInfoResult.IsSuccess)
        {
            return Result<IPacket>.FromError(packetInfoResult);
        }

        var packetInfo = packetInfoResult.Entity;
        var deserializedResult = packetInfo.PacketConverter.Deserialize(ref packetStringEnumerator, default);
        if (!deserializedResult.IsSuccess)
        {
            return Result<IPacket>.FromError(deserializedResult);
        }

        var packet = deserializedResult.Entity as IPacket;

        if (packet is null)
        {
            return new DeserializedValueNullError(packetInfo.PacketType);
        }

        return Result<IPacket>.FromSuccess(packet);
    }
}