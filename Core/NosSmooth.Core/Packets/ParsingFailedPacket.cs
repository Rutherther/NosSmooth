//
//  ParsingFailedPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosCore.Packets;
using Remora.Results;

namespace NosSmooth.Core.Packets;

/// <summary>
/// Represents packet that failed to parse correctly.
/// </summary>
public class ParsingFailedPacket : PacketBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParsingFailedPacket"/> class.
    /// </summary>
    /// <param name="serializerResult">The result from the serializer.</param>
    /// <param name="packet">The full text of the packet.</param>
    public ParsingFailedPacket(IResult serializerResult, string packet)
    {
        SerializerResult = serializerResult;
        Packet = packet;
    }

    /// <summary>
    /// Gets the result from the serializer.
    /// </summary>
    public IResult SerializerResult { get; }

    /// <summary>
    /// Gets he full packet string.
    /// </summary>
    public string Packet { get; }
}