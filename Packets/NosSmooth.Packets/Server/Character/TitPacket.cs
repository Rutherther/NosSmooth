//
//  TitPacket.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Abstractions.Common;

namespace NosSmooth.Packets.Server.Character;

/// <summary>
/// A packet containing translated class name and the character's name.
/// </summary>
/// <remarks>
/// Sent during initialization.
/// I do not know what purpose of this packet is
/// as the client has all the information from
/// this packet already.
/// </remarks>
/// <param name="TranslatedClass">The class translated in client language.</param>
/// <param name="Name">The name of the client character.</param>
[PacketHeader("tit", PacketSource.Server)]
[GenerateSerializer(true)]
public record TitPacket
(
    [PacketIndex(0)]
    string TranslatedClass,
    [PacketGreedyIndex(1)]
    NameString Name
) : IPacket;