//
//  PacketGreedyIndexAttribute.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.PacketSerializer.Abstractions.Attributes;

/// <summary>
/// Attribute for marking packet parameters greedy (read to the last token).
/// </summary>
public class PacketGreedyIndexAttribute : PacketIndexAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PacketGreedyIndexAttribute"/> class.
    /// </summary>
    /// <param name="index">The position.</param>
    public PacketGreedyIndexAttribute(ushort index)
        : base(index)
    {
    }
}