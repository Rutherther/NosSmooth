//
//  PacketIndexAttribute.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace NosSmooth.Packets.Attributes;

/// <summary>
/// Attribute for marking properties in packets with their position in the packet.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public class PacketIndexAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PacketIndexAttribute"/> class.
    /// </summary>
    /// <param name="index">The position of the property.</param>
    public PacketIndexAttribute(ushort index)
    {
        Index = index;
    }

    /// <summary>
    /// Gets the index of the current property.
    /// </summary>
    public ushort Index { get; }

    /// <summary>
    /// Gets the inner separator used for complex types such as sub packets.
    /// </summary>
    public char? InnerSeparator { get; set; }

    /// <summary>
    /// Gets the separator after this field.
    /// </summary>
    public char? AfterSeparator { get; set; }
}