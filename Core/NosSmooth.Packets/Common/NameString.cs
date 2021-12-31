//
//  NameString.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net.NetworkInformation;
using System.Reflection;

namespace NosSmooth.Packets.Common;

/// <summary>
/// Represents name in the game replacing "^" for " ".
/// </summary>
public class NameString
{
    /// <summary>
    /// Gets the character used to separate words.
    /// </summary>
    public static char WordSeparator => '^';

    /// <summary>
    /// Creates <see cref="NameString"/> instance from the given name retrieved from a packet.
    /// </summary>
    /// <param name="packetName">The name from the packet.</param>
    /// <returns>A name string instance.</returns>
    public static NameString FromPacket(string packetName)
    {
        return new NameString(packetName, true);
    }

    /// <summary>
    /// Creates <see cref="NameString"/> instance from the given name retrieved from a packet.
    /// </summary>
    /// <param name="packetName">The name from the packet.</param>
    /// <returns>A name string instance.</returns>
    public static NameString FromString(string packetName)
    {
        return new NameString(packetName, true);
    }

    private NameString(string name, bool packet)
    {
        if (packet)
        {
            PacketName = name;
            Name = name.Replace(WordSeparator, ' ');
        }
        else
        {
            Name = name;
            PacketName = name.Replace(' ', WordSeparator);
        }
    }

    /// <summary>
    /// The real name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The original name in the packet.
    /// </summary>
    public string PacketName { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        return Name;
    }

    /// <summary>
    /// Converts name string to regular string.
    /// Returns the real name.
    /// </summary>
    /// <param name="nameString">The name string to convert.</param>
    /// <returns>The real name.</returns>
    public static implicit operator string(NameString nameString)
    {
        return nameString.Name;
    }

    /// <summary>
    /// Converts regular string to name string.
    /// </summary>
    /// <param name="name">The string to convert.</param>
    /// <returns>The name string.</returns>
    public static implicit operator NameString(string name)
    {
        return FromString(name);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (!(obj is NameString nameString))
        {
            return false;
        }

        return Name.Equals(nameString.Name);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}