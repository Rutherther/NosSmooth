﻿//
//  PacketListIndexAttribute.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.PacketSerializer.Abstractions.Attributes;

/// <summary>
/// Attribute for marking property as a packet list.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class PacketListIndexAttribute : PacketIndexAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PacketListIndexAttribute"/> class.
    /// </summary>
    /// <param name="index">The position in the packet.</param>
    public PacketListIndexAttribute(ushort index)
        : base(index)
    {
    }

    /// <summary>
    /// Gets or sets the separator of the items in the array.
    /// </summary>
    public char ListSeparator { get; set; } = '|';

    /// <summary>
    /// Gets or sets the length of the array.
    /// </summary>
    public uint Length { get; set; } = 0;
}