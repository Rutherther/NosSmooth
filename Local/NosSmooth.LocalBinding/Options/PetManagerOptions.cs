//
//  PetManagerOptions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.LocalBinding.Objects;

namespace NosSmooth.LocalBinding.Options;

/// <summary>
/// Options for <see cref="PetManagerBinding"/>.
/// </summary>
public class PetManagerOptions
{
    /// <summary>
    /// Gets or sets the pattern to find static pet manager list address at.
    /// </summary>
    public string PetManagerListPattern { get; set; }
        = "00 ff 70 00 ?? ?? ?? ?? 10 90";

    /// <summary>
    /// Gets or sets the offsets to find the pet manager list at from the static address.
    /// </summary>
    public int[] PetManagerListOffsets { get; set; }
        = { 4, 8, 0x180, 0x18, 0x1C, 0x54 };
}