//
//  Constants.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NosSmooth.PacketSerializersGenerator;

/// <summary>
/// Contains constants needed for the generation.
/// </summary>
public class Constants
{
    /// <summary>
    /// Gets the full name of the generate source attribute class.
    /// </summary>
    public static string GenerateSourceAttributeClass => "NosSmooth.Packets.Attributes.GenerateSerializerAttribute";

    /// <summary>
    /// Gets the full name of the packet attribute classes that are used for the generation.
    /// </summary>
    public static string PacketAttributesClassRegex => @"^NosSmooth\.Packets\.Attributes\.Packet.*";

    /// <summary>
    /// Gets the full name of helper class used for inline type converters.
    /// </summary>
    public static string HelperClass => "NosSmooth.Generated.HelperClass";
}