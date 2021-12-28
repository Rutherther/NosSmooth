//
//  PacketTypesRepository.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NosSmooth.Packets.Attributes;
using NosSmooth.Packets.Converters;
using NosSmooth.Packets.Errors;
using Remora.Results;

namespace NosSmooth.Packets.Packets;

/// <summary>
/// Repository of packet types for finding information about packets.
/// </summary>
public class PacketTypesRepository
{
    private readonly TypeConverterRepository _typeConverterRepository;
    private readonly Dictionary<PacketSource, Dictionary<string, PacketInfo>> _headerToPacket;
    private readonly Dictionary<string, PacketInfo> _typeToPacket;

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketTypesRepository"/> class.
    /// </summary>
    /// <param name="typeConverterRepository">The type converter repository.</param>
    public PacketTypesRepository(TypeConverterRepository typeConverterRepository)
    {
        _typeConverterRepository = typeConverterRepository;
        _headerToPacket = new Dictionary<PacketSource, Dictionary<string, PacketInfo>>();
        _typeToPacket = new Dictionary<string, PacketInfo>();
    }

    /// <summary>
    /// Add the given packet type to the repository.
    /// </summary>
    /// <param name="type">The type of the packet.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result AddPacketType(Type type)
    {
        if (typeof(IPacket).IsAssignableFrom(type))
        {
            return new ArgumentInvalidError(nameof(type), "The type has to be assignable to IPacket.");
        }

        var header = type.GetCustomAttribute<PacketHeaderAttribute>();
        if (header is null)
        {
            return new ArgumentInvalidError(nameof(type), "Every packet has to specify the header.");
        }

        var converterResult = _typeConverterRepository.GetTypeConverter(type);
        if (!converterResult.IsSuccess)
        {
            return Result.FromError(converterResult);
        }

        var info = new PacketInfo(header.Identifier, type, converterResult.Entity);

        if (_headerToPacket.ContainsKey(header.Source))
        {
            _headerToPacket[header.Source] = new Dictionary<string, PacketInfo>();
        }

        if (type.FullName is not null)
        {
            _typeToPacket[type.FullName] = info;
        }

        if (header.Identifier is not null)
        {
            if (_headerToPacket[header.Source].ContainsKey(header.Identifier))
            {
                return new AmbiguousHeaderError
                (
                    header.Identifier,
                    header.Source,
                    new[] { _headerToPacket[header.Source][header.Identifier], info }
                );
            }

            _headerToPacket[header.Source][header.Identifier] = info;
        }

        return Result.FromSuccess();
    }

    /// <summary>
    /// Gets the type of a packet that corresponds to the given header.
    /// </summary>
    /// <param name="header">The header of the packet.</param>
    /// <param name="preferredSource">The preferred source, first this source will be checked for the header and if packet with that source is not found, other sources will be accpeted as well.</param>
    /// <returns>Info that stores the packet's info. Or an error, if not found.</returns>
    public Result<PacketInfo> FindPacketInfo(string header, PacketSource preferredSource)
    {
        if (_headerToPacket[preferredSource].ContainsKey(header))
        {
            return _headerToPacket[preferredSource][header];
        }

        var foundPackets = _headerToPacket.Values
            .Where(x => x.ContainsKey(header))
            .Select(x => x[header]).ToArray();

        return foundPackets.Length switch
        {
            1 => foundPackets[0],
            0 => new PacketConverterNotFoundError(header),
            _ => new AmbiguousHeaderError(header, null, foundPackets)
        };
    }

    /// <summary>
    /// Gets the packet info from the given packet type.
    /// </summary>
    /// <typeparam name="TPacket">The type of the packet.</typeparam>
    /// <returns>Info that stores the packet's info. Or an error, if not found.</returns>
    public Result<PacketInfo> FindPacketInfo<TPacket>() where TPacket : IPacket
        => FindPacketInfo(typeof(TPacket));

    /// <summary>
    /// Gets the packet info from the given packet type.
    /// </summary>
    /// <param name="packetType">The type of the packet.</param>
    /// <returns>Info that stores the packet's info. Or an error, if not found.</returns>
    public Result<PacketInfo> FindPacketInfo(Type packetType)
        => packetType.FullName is null
            ? new ArgumentInvalidError(nameof(packetType), "The type has to have a name to get packet info.")
            : FindPacketInfoByTypeName(packetType.FullName);

    /// <summary>
    /// Gets the packet info from the given packet type name.
    /// </summary>
    /// <param name="packetTypeFullName">The full name of the type of the packet.</param>
    /// <returns>Info that stores the packet's info. Or an error, if not found.</returns>
    public Result<PacketInfo> FindPacketInfoByTypeName(string packetTypeFullName)
    {
        if (!_typeToPacket.ContainsKey(packetTypeFullName))
        {
            return new PacketConverterNotFoundError(packetTypeFullName);
        }

        return _typeToPacket[packetTypeFullName];
    }
}