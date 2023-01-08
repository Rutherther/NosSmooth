//
//  PacketTypesRepository.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NosSmooth.Packets;
using NosSmooth.PacketSerializer.Abstractions;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Errors;
using Remora.Results;

namespace NosSmooth.PacketSerializer.Packets;

/// <summary>
/// Repository of packet types for finding information about packets.
/// </summary>
public class PacketTypesRepository : IPacketTypesRepository
{
    private readonly IStringConverterRepository _stringConverterRepository;
    private readonly Dictionary<PacketSource, Dictionary<string, PacketInfo>> _headerToPacket;
    private readonly Dictionary<string, PacketInfo> _typeToPacket;

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketTypesRepository"/> class.
    /// </summary>
    /// <param name="stringConverterRepository">The type converter repository.</param>
    public PacketTypesRepository(IStringConverterRepository stringConverterRepository)
    {
        _stringConverterRepository = stringConverterRepository;
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
        if (!typeof(IPacket).IsAssignableFrom(type))
        {
            return new ArgumentInvalidError
                (nameof(type), $"The type has to be assignable to IPacket. {type.FullName} isn't.");
        }

        var headers = type.GetCustomAttributes<PacketHeaderAttribute>().ToList();
        if (headers.Count == 0)
        {
            return new ArgumentInvalidError
            (
                nameof(type),
                $"Every packet has to have a header specified using [PacketHeader] attribute. {type.FullName} doesn't have a header."
            );
        }

        var converterResult = _stringConverterRepository.GetTypeConverter(type);
        if (!converterResult.IsSuccess)
        {
            return Result.FromError(converterResult);
        }

        if (headers.Count == 1)
        {
            return AddPacket(headers[0], type, converterResult.Entity);
        }

        var errors = new List<IResult>();
        foreach (var header in headers)
        {
            var result = AddPacket(header, type, converterResult.Entity);
            if (!result.IsSuccess)
            {
                errors.Add(result);
            }
        }

        return errors.Count switch
        {
            0 => Result.FromSuccess(),
            1 => (Result)errors[0],
            _ => new AggregateError(errors)
        };
    }

    private Result AddPacket(PacketHeaderAttribute header, Type type, IStringConverter converter)
    {
        var info = new PacketInfo(header.Identifier, type, converter);
        if (type.FullName is not null)
        {
            if (_typeToPacket.ContainsKey(type.FullName))
            { // The packet was already added.
                return Result.FromSuccess();
            }

            _typeToPacket[type.FullName] = info;
        }

        if (!_headerToPacket.ContainsKey(header.Source))
        {
            _headerToPacket[header.Source] = new Dictionary<string, PacketInfo>();
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

    /// <inheritdoc />
    public Result AddPacketTypes(IEnumerable<Type> packetTypes)
    {
        var errorResults = new List<IResult>();
        foreach (var packetType in packetTypes)
        {
            var result = AddPacketType(packetType);
            if (!result.IsSuccess)
            {
                errorResults.Add(result);
            }
        }

        return errorResults.Count switch
        {
            0 => Result.FromSuccess(),
            1 => (Result)errorResults[0],
            _ => new AggregateError(errorResults)
        };
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

        PacketInfo? info = null;
        foreach (var dict in _headerToPacket.Values)
        {
            if (dict.ContainsKey(header))
            {
                if (info is null)
                {
                    info = dict[header];
                }
                else
                {
                    return new AmbiguousHeaderError(header, null, new PacketInfo[] { dict[header], info });
                }
            }
        }

        if (info is null)
        {
            return new PacketConverterNotFoundError(header);
        }

        return info;
    }

    /// <summary>
    /// Gets the packet info from the given packet type.
    /// </summary>
    /// <typeparam name="TPacket">The type of the packet.</typeparam>
    /// <returns>Info that stores the packet's info. Or an error, if not found.</returns>
    public Result<PacketInfo> FindPacketInfo<TPacket>()
        where TPacket : IPacket
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