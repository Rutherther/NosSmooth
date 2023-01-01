//
//  PacketTypesRepositoryExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Reflection;
using NosSmooth.Packets;
using NosSmooth.PacketSerializer.Packets;
using Remora.Results;

namespace NosSmooth.PacketSerializer.Extensions;

/// <summary>
/// Extension methods for <see cref="IPacketTypesRepository"/>.
/// </summary>
public static class PacketTypesRepositoryExtensions
{
    /// <summary>
    /// Add packets from the given assembly.
    /// </summary>
    /// <param name="packetTypesRepository">The packet types repository.</param>
    /// <param name="assembly">The assembly to add packets from.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public static Result AddPacketTypes(this IPacketTypesRepository packetTypesRepository, Assembly assembly)
    {
        var packetTypes = assembly
            .GetExportedTypes()
            .Where(x => x != typeof(UnresolvedPacket) && !x.IsAbstract && typeof(IPacket).IsAssignableFrom(x));
        return packetTypesRepository.AddPacketTypes(packetTypes);
    }

    /// <summary>
    /// Adds the default NosSmooth packets.
    /// </summary>
    /// <param name="packetTypesRepository">The packet types repository.</param>
    /// <returns>A result tht may or may not have succeeded.</returns>
    public static Result AddDefaultPackets(this IPacketTypesRepository packetTypesRepository)
    {
        return packetTypesRepository.AddPacketTypes(typeof(IPacket).Assembly);
    }
}