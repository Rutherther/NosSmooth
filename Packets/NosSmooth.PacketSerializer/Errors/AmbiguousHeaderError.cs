//
//  AmbiguousHeaderError.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using NosSmooth.PacketSerializer.Abstractions.Attributes;
using NosSmooth.PacketSerializer.Packets;
using Remora.Results;

namespace NosSmooth.PacketSerializer.Errors;

/// <summary>
/// The header was ambiguous, there were at least two packets with the same header and source.
/// </summary>
/// <param name="Header">The packet's header.</param>
/// <param name="Source">The packet's source.</param>
/// <param name="PacketTypes">The types that were ambiguous.</param>
public record AmbiguousHeaderError(string Header, PacketSource? Source, IReadOnlyList<PacketInfo> PacketTypes)
    : ResultError($"There was more than one packet with the header {Header} in the {Source.ToString() ?? "Unknown"} source. ({string.Join(", ", PacketTypes.Select(x => x.PacketType.FullName))})");