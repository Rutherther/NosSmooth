//
//  IPacketTypesRepository.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using NosSmooth.Packets;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using Remora.Results;

namespace NosSmooth.PacketSerializer.Packets;

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
    /// Add all of the given packet types.
    /// </summary>
    /// <remarks>
    /// If there is an error, it will continue to add the rest of the types
    /// and then return an aggregate error containing all the errors.
    ///
    /// The application can still run, but without the errorful packets.
    /// </remarks>
    /// <param name="packetTypes">The types add.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result AddPacketTypes(IEnumerable<Type> packetTypes);

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