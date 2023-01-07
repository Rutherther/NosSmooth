//
//  DeserializeOptions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace NosSmooth.PacketSerializer.Abstractions;

/// <summary>
/// Options for deserialization.
/// </summary>
/// <param name="CanBeNull">Whether the argument may be null.</param>
[SuppressMessage
(
    "StyleCop.CSharp.NamingRules",
    "SA1313:Parameter names should begin with lower-case letter",
    Justification = "Fix this."
)]
public record struct DeserializeOptions(bool CanBeNull = false)
{
    /// <summary>
    /// Gets the nullable deserialize options.
    /// </summary>
    public static DeserializeOptions Nullable => new DeserializeOptions(true);
}