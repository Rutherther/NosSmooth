//
//  PetManagerOptions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.LocalBinding.Objects;
using NosSmooth.LocalBinding.Structs;

namespace NosSmooth.LocalBinding.Options;

/// <summary>
/// Options for <see cref="PetManagerList"/>.
/// </summary>
public class PetManagerOptions
{
    /// <summary>
    /// Gets or sets the pattern to find static pet manager list address at.
    /// </summary>
    public string PetManagerListPattern { get; set; }
        = "8B F8 8B D3 A1 ?? ?? ?? ?? E8 ?? ?? ?? ?? 8B D0";

    /// <summary>
    /// Gets or sets the offsets to find the pet manager list at from the static address.
    /// </summary>
    public int[] PetManagerListOffsets { get; set; }
        = { 5, 0 };
}