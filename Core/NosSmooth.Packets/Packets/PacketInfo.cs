//
//  PacketInfo.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NosSmooth.Packets.Converters;

namespace NosSmooth.Packets.Packets;

/// <summary>
/// Information about a packet type.
/// </summary>
/// <param name="Header">The packet's header, if any.</param>
/// <param name="PacketType">The packet's type.</param>
/// <param name="PacketConverter">The packet's converter.</param>
public record PacketInfo(string? Header, Type PacketType, ITypeConverter PacketConverter);