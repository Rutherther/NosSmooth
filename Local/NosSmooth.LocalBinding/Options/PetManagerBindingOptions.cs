//
//  PetManagerBindingOptions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.LocalBinding.Objects;

namespace NosSmooth.LocalBinding.Options;

/// <summary>
/// Options for <see cref="PetManagerBinding"/>.
/// </summary>
public class PetManagerBindingOptions
{
    /// <summary>
    /// Gets or sets the pattern of a pet walk function.
    /// </summary>
    public string PetWalkPattern { get; set; }
        = "55 8b ec 83 c4 e4 53 56 57 8b f9 89 55 fc 8b d8 c6 45 fb 00";

    /// <summary>
    /// Gets or sets whether to hook the pet walk function.
    /// </summary>
    public bool HookPetWalk { get; set; } = true;
}