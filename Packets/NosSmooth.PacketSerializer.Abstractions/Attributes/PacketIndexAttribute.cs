//
//  PacketIndexAttribute.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.PacketSerializer.Abstractions.Attributes;

/// <summary>
/// Attribute for marking properties in packets with their position in the packet.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
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
    public char InnerSeparator { get; set; } = (char)0xFF;

    /// <summary>
    /// Gets the separator after this field.
    /// </summary>
    public char AfterSeparator { get; set; } = (char)0xFF;

    /// <summary>
    /// Gets or sets whether this parameter is optional.
    /// </summary>
    /// <remarks>
    /// Optional attributes have to be nullable,
    /// if the attribute is not in the string,
    /// it will be set to null. For serializer,
    /// if the parameter is null, it will be omitted.
    ///
    /// See <see cref="PacketConditionalIndexAttribute"/> for
    /// more complex decision making about using parameters.
    /// </remarks>
    public bool IsOptional { get; set; } = false;

    /// <summary>
    /// Gets or sets whether there may be multiple separators after the field.
    /// </summary>
    public bool AllowMultipleSeparators { get; set; } = false;

    /// <summary>
    /// Gets or sets the number of multiple separators after the field.
    /// </summary>
    /// <remarks>
    /// <see cref="AllowMultipleSeparators"/> has to be true to make this work.
    /// </remarks>
    public short MultipleSeparatorsCount { get; set; } = 1;
}