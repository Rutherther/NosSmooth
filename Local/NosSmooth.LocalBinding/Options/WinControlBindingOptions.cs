//
//  WinControlBindingOptions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.LocalBinding.Objects;

namespace NosSmooth.LocalBinding.Options;

/// <summary>
/// Options for <see cref="WinControlBinding"/>.
/// </summary>
public class WinControlBindingOptions
{
    /// <summary>
    /// Gets or sets whether to hook MainWndProc.
    /// </summary>
    public bool HookMainWndProc { get; set; } = false;

    /// <summary>
    /// Gets or sets pattern to find main wnd proc at.
    /// </summary>
    public string MainWndProcPattern { get; set; }
        = "55 8b ec 51 53 56 57 89 45 fc 33 c0 55 ?? ?? ?? ?? ?? 64 ff 30 64 89 20 33 c0 55 ?? ?? ?? ?? ?? 64 FF 30 64 89 20 8B 5D FC 8B 43 3C FF 53 38 33 C0 5A 59 59 64 89 10";
}