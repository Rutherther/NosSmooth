//
//  ISpecificPacketSerializer.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using NosCore.Packets.Attributes;
using NosCore.Packets.Interfaces;
using Remora.Results;

namespace NosSmooth.Core.Packets.Converters;

/// <summary>
/// Converts packets that cannot be handled easily by the default serializer.
/// </summary>
public interface ISpecificPacketSerializer
{
    /// <summary>
    /// Gets whether this is a serializer.
    /// </summary>
    public bool Serializer { get; }

    /// <summary>
    /// Gets whether this is a deserializer.
    /// </summary>
    public bool Deserializer { get; }

    /// <summary>
    /// Whether the current packet serializer should handle the packet.
    /// </summary>
    /// <param name="packetString">The string of the packet.</param>
    /// <returns>If this serializer should be used for handling.</returns>
    public bool ShouldHandle(string packetString);

    /// <summary>
    /// Whether the current packet serializer should handle the packet.
    /// </summary>
    /// <param name="packet">The packet object.</param>
    /// <returns>If this serializer should be used for handling.</returns>
    public bool ShouldHandle(IPacket packet);

    /// <summary>
    /// Serialize the given packet into string.
    /// </summary>
    /// <param name="packet">The string of the packet.</param>
    /// <returns>The serialized packet or an error.</returns>
    public Result<string> Serialize(IPacket packet);

    /// <summary>
    /// Deserialize the given packet to its type.
    /// </summary>
    /// <param name="packetString">The string of the packet.</param>
    /// <returns>The deserialized packet or an error.</returns>
    public Result<IPacket> Deserialize(string packetString);
}

/// <summary>
/// Converts packets that cannot be handled easily by the default serializer.
/// </summary>
/// <typeparam name="TPacket">The packet.</typeparam>
public abstract class SpecificPacketSerializer<TPacket> : ISpecificPacketSerializer
    where TPacket : IPacket
{
    private string? _packetHeader;

    /// <summary>
    /// Gets the packet header identifier.
    /// </summary>
    public string PacketHeader
    {
        get
        {
            if (_packetHeader is null)
            {
                _packetHeader = typeof(TPacket).GetCustomAttribute<PacketHeaderAttribute>()!.Identification + " ";
            }

            return _packetHeader;
        }
    }

    /// <inheritdoc />
    public abstract bool Serializer { get; }

    /// <inheritdoc />
    public abstract bool Deserializer { get; }

    /// <inheritdoc />
    public bool ShouldHandle(string packetString)
    {
        return packetString.StartsWith(PacketHeader);
    }

    /// <inheritdoc />
    public bool ShouldHandle(IPacket packet)
    {
        return typeof(TPacket) == packet.GetType();
    }

    /// <inheritdoc/>
    Result<string> ISpecificPacketSerializer.Serialize(IPacket packet)
    {
        return Serialize((TPacket)packet);
    }

    /// <inheritdoc/>
    Result<IPacket> ISpecificPacketSerializer.Deserialize(string packetString)
    {
        var result = Deserialize(packetString);
        if (!result.IsSuccess)
        {
            return Result<IPacket>.FromError(result);
        }

        return Result<IPacket>.FromSuccess(result.Entity);
    }

    /// <summary>
    /// Serialize the given packet into string.
    /// </summary>
    /// <param name="packet">The string of the packet.</param>
    /// <returns>The serialized packet or an error.</returns>
    public abstract Result<string> Serialize(TPacket packet);

    /// <summary>
    /// Deserialize the given packet to its type.
    /// </summary>
    /// <param name="packetString">The string of the packet.</param>
    /// <returns>The deserialized packet or an error.</returns>
    public abstract Result<TPacket> Deserialize(string packetString);
}