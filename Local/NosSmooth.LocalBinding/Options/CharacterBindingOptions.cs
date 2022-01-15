//
//  CharacterBindingOptions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.LocalBinding.Objects;

namespace NosSmooth.LocalBinding.Options;

/// <summary>
/// Options for <see cref="CharacterBinding"/>.
/// </summary>
public class CharacterBindingOptions
{
    /// <summary>
    /// Gets or sets whether to hook the walk function.
    /// </summary>
    public bool HookWalk { get; set; } = true;

    /// <summary>
    /// Gets or sets the pattern to find the character object at.
    /// </summary>
    /// <remarks>
    /// The address of the object is "three pointers down" from address found on this pattern.
    /// </remarks>
    public string CharacterObjectPattern { get; set; }
        = "33 C9 8B 55 FC A1 ?? ?? ?? ?? E8 ?? ?? ?? ??";

    /// <summary>
    /// Gets or sets the pattern to find the walk function at.
    /// </summary>
    public string WalkFunctionPattern { get; set; } = "55 8B EC 83 C4 EC 53 56 57 66 89 4D FA";
}