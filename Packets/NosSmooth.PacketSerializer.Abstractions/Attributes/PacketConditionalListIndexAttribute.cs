//
//  PacketConditionalListIndexAttribute.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.PacketSerializer.Abstractions.Attributes;

/// <summary>
/// <see cref="PacketConditionalIndexAttribute"/> + <see cref="PacketListIndexAttribute"/>
/// in one.
/// </summary>
public class PacketConditionalListIndexAttribute : PacketListIndexAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PacketConditionalListIndexAttribute"/> class.
    /// You can use this attribute multiple times on one parameter.
    /// </summary>
    /// <param name="index">The position in the packet.</param>
    /// <param name="conditionParameter">What parameter to check. (it has to precede this one).</param>
    /// <param name="negate">Whether to negate the match values (not equals).</param>
    /// <param name="matchValues">The values that mean this parameter is present.</param>
    public PacketConditionalListIndexAttribute(ushort index, string conditionParameter, bool negate = false, params object?[] matchValues)
        : base(index)
    {
    }
}