//
//  PacketHeaderAttribute.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace NosSmooth.Packets.Attributes;

/// <summary>
/// Attribute for specifying the header identifier of the packet.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class PacketHeaderAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PacketHeaderAttribute"/> class.
    /// </summary>
    /// <param name="identifier">The packet identifier (the first entry).</param>
    /// <param name="source">The source of the packet.</param>
    public PacketHeaderAttribute(string? identifier, PacketSource source)
    {
        Identifier = identifier;
        Source = source;
    }

    /// <summary>
    /// Gets the identifier of the packet (the first entry in the packet).
    /// </summary>
    public string? Identifier { get; }

    /// <summary>
    /// Gets the source of the packet used for determining where the packet is from.
    /// </summary>
    public PacketSource Source { get; }
}