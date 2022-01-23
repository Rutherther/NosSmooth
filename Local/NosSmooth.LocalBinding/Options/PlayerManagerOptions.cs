//
//  PlayerManagerOptions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.LocalBinding.Structs;

namespace NosSmooth.LocalBinding.Options;

/// <summary>
/// Options for <see cref="PlayerManager"/>.
/// </summary>
public class PlayerManagerOptions
{
    /// <summary>
    /// Gets or sets the pattern to find the character object at.
    /// </summary>
    public string PlayerManagerPattern { get; set; }
        = "33 C9 8B 55 FC A1 ?? ?? ?? ?? E8 ?? ?? ?? ??";

    /// <summary>
    /// Gets or sets the offsets to find the player manager at from the static address.
    /// </summary>
    public int[] PlayerManagerOffsets { get; set; }
        = { 6, 0 };
}