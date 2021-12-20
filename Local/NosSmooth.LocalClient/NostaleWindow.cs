//
//  NostaleWindow.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.LocalClient.Utils;

namespace NosSmooth.LocalClient;

/// <summary>
/// Represents window of nostale client.
/// </summary>
public class NostaleWindow
{
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int WM_CHAR = 0x0102;

    /// <summary>
    /// Initializes a new instance of the <see cref="NostaleWindow"/> class.
    /// </summary>
    /// <param name="handle">The handle of the window.</param>
    public NostaleWindow(IntPtr handle) => Handle = handle;

    /// <summary>
    /// Gets the window handle.
    /// </summary>
    public IntPtr Handle { get; }

    /// <summary>
    /// Changes the title of the window.
    /// </summary>
    /// <param name="name">The new name of the window.</param>
    public void Rename(string name)
    {
        User32.SetWindowText(Handle, name);
    }

    /// <summary>
    /// Bring the window to front.
    /// </summary>
    public void BringToFront()
    {
        User32.SetForegroundWindow(Handle);
    }

    /// <summary>
    /// Send the given key to the window.
    /// </summary>
    /// <param name="key">The id of the key.</param>
    public void SendKey(uint key)
    {
        User32.PostMessage(Handle, WM_KEYDOWN, key, 0);
        User32.PostMessage(Handle, WM_CHAR, key, 0);
        User32.PostMessage(Handle, WM_KEYUP, key, 0);
    }
}