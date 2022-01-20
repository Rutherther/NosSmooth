//
//  SceneManagerBindingOptions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.LocalBinding.Objects;

namespace NosSmooth.LocalBinding.Options;

/// <summary>
/// Options for <see cref="SceneManagerBinding"/>.
/// </summary>
public class SceneManagerBindingOptions
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
    /// Gets or sets the pattern to find the focus entity method at.
    /// </summary>
    public string FocusEntityPattern { get; set; }
        = "73 00 00 00 55 8b ec b9 05 00 00 00";

    /// <summary>
    /// Gets or sets whether to hook the Focus entity function.
    /// </summary>
    public bool HookFocusEntity { get; set; } = true;
}