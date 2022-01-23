//
//  SceneManagerOptions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.LocalBinding.Structs;

namespace NosSmooth.LocalBinding.Options;

/// <summary>
/// Options for <see cref="SceneManager"/>.
/// </summary>
public class SceneManagerOptions
{
    /// <summary>
    /// Gets or sets the pattern to find the scene manager object at.
    /// </summary>
    /// <remarks>
    /// The address of the object is direct pointer to the scene manager.
    /// </remarks>
    public string SceneManagerObjectPattern { get; set; }
        = "FF ?? ?? ?? ?? ?? FF FF FF 00 00 00 00 00 00 00 00 00 00 00 00 FF FF FF FF";

    /// <summary>
    /// Gets or sets the offsets to find the scene manager at from the static address.
    /// </summary>
    public int[] SceneManagerOffsets { get; set; }
        = { 1 };
}