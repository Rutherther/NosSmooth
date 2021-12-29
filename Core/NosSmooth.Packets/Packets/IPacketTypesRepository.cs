//
//  IPacketTypesRepository.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using NosSmooth.Packets.Attributes;
using NosSmooth.Packets.Errors;
using Remora.Results;

namespace NosSmooth.Packets.Packets;

/// <summary>
/// Repository of packet types for finding information about packets.
/// </summary>
public interface IPacketTypesRepository
{
    /// <summary>
    /// Add the given packet type to the repository.
    /// </summary>
    /// <param name="type">The type of the packet.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result AddPacketType(Type type);

    /// <summary>
    /// Gets the type of a packet that corresponds to the given header.
    /// </summary>
    /// <param name="header">The header of the packet.</param>
    /// <param name="preferredSource">The preferred source, first this source will be checked for the header and if packet with that source is not found, other sources will be accpeted as well.</param>
    /// <returns>Info that stores the packet's info. Or an error, if not found.</returns>
    public Result<PacketInfo> FindPacketInfo(string header, PacketSource preferredSource);

    /// <summary>
    /// Gets the packet info from the given packet type.
    /// </summary>
    /// <typeparam name="TPacket">The type of the packet.</typeparam>
    /// <returns>Info that stores the packet's info. Or an error, if not found.</returns>
    public Result<PacketInfo> FindPacketInfo<TPacket>()
        where TPacket : IPacket;

    /// <summary>
    /// Gets the packet info from the given packet type.
    /// </summary>
    /// <param name="packetType">The type of the packet.</param>
    /// <returns>Info that stores the packet's info. Or an error, if not found.</returns>
    public Result<PacketInfo> FindPacketInfo(Type packetType);

    /// <summary>
    /// Gets the packet info from the given packet type name.
    /// </summary>
    /// <param name="packetTypeFullName">The full name of the type of the packet.</param>
    /// <returns>Info that stores the packet's info. Or an error, if not found.</returns>
    public Result<PacketInfo> FindPacketInfoByTypeName(string packetTypeFullName);
}