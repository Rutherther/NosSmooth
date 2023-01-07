//
//  NullableWrapper.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace NosSmooth.PacketSerializer.Abstractions;

/// <summary>
/// Wraps a compound value that may not be present
/// and there will be "-1" instead in the packet.
/// The converter of underlying type will be called
/// if and only if the value is not null.
/// </summary>
/// <param name="Value">The value.</param>
/// <typeparam name="T">The underlying type.</typeparam>
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Fix this, this should not happen.")]
public record struct NullableWrapper<T>(T? Value)
{
    /// <summary>
    /// Unwrap the underlying value.
    /// </summary>
    /// <param name="wrapper">The wrapper to unwrap.</param>
    /// <returns>The unwrapped value.</returns>
    public static implicit operator T?(NullableWrapper<T> wrapper)
    {
        return wrapper.Value;
    }

    public static implicit operator NullableWrapper<T>(T? value)
    {
        return new NullableWrapper<T>(value);
    }
}