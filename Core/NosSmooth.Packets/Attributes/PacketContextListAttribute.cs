//
//  PacketContextListAttribute.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace NosSmooth.Packets.Attributes;

/// <summary>
/// Attribute for marking properties as a contextual list.
/// </summary>
/// <remarks>
/// Contextual list gets its length from another property that was already set.
/// </remarks>
[AttributeUsage(AttributeTargets.Parameter)]
public class PacketContextListAttribute : PacketListIndexAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PacketContextListAttribute"/> class.
    /// </summary>
    /// <param name="index">The position in the packet.</param>
    /// <param name="lengthStoredIn">The name of the property the length is stored in.</param>
    public PacketContextListAttribute(ushort index, string lengthStoredIn)
        : base(index)
    {
        LengthStoredIn = lengthStoredIn;
    }

    /// <summary>
    /// Gets or sets the attribute name that stores the length of this list.
    /// </summary>
    public string LengthStoredIn { get; set; }
}