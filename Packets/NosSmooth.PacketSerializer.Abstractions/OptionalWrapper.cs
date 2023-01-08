//
//  OptionalWrapper.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace NosSmooth.PacketSerializer.Abstractions;

/// <summary>
/// Wraps a compound value that may not be present
/// and there will be nothing instead in the packet.
/// The converter of underlying type will be called
/// if and only if the value is not null.
/// </summary>
/// <param name="Present">Whether the value is present in the packet. (to differentiate between null and not present.)</param>
/// <param name="Value">The value.</param>
/// <typeparam name="T">The underlying type.</typeparam>
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Fix this, this should not happen.")]
public record struct OptionalWrapper<T>(bool Present, T? Value)
{
    /// <summary>
    /// Unwrap the underlying value.
    /// </summary>
    /// <param name="wrapper">The wrapper to unwrap.</param>
    /// <returns>The unwrapped value.</returns>
    public static implicit operator T?(OptionalWrapper<T> wrapper)
    {
        return wrapper.Value;
    }

    /// <summary>
    /// wrap the value in optional wrapper.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <returns>The wrapped value.</returns>
    public static implicit operator OptionalWrapper<T>(T? value)
    {
        return new OptionalWrapper<T>(true, value);
    }
}