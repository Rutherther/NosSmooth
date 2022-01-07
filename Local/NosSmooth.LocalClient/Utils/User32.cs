//
//  User32.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
#pragma warning disable CS1591

namespace NosSmooth.LocalClient.Utils;

/// <summary>
/// Represents class with extern calls to user32.dll.
/// </summary>
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600", MessageId = "Elements should be documented", Justification = "user32.dll methods do not need documentation, it can be found on msdn.")]
public class User32
{
    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern int PostMessage(IntPtr hWnd, int uMsg, uint wParam, uint lParam);

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool SetWindowText(IntPtr hWnd, string text);

    [DllImport("user32.dll")]
    public static extern int EnumWindows(EnumWindowsProc callback, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    /// <summary>
    /// Finds all windows with the given title matching.
    /// </summary>
    /// <param name="title">The title to match.</param>
    /// <returns>The matched windows.</returns>
    public static IEnumerable<IntPtr> FindWindowsWithTitle(string title)
    {
        var windows = new List<IntPtr>();
        EnumWindows(
            (hWnd, lParam) =>
            {
                string windowTitle = GetWindowTitle(hWnd);
                if (windowTitle.Equals(title))
                {
                    windows.Add(hWnd);
                }

                return true;
            },
            IntPtr.Zero
        );

        return windows;
    }

    /// <summary>
    /// Returns the title of a window.
    /// </summary>
    /// <param name="hWnd">The handle of the window.</param>
    /// <returns>The title of the window.</returns>
    public static string GetWindowTitle(IntPtr hWnd)
    {
        int size = GetWindowTextLength(hWnd);
        if (size == 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder(size + 1);
        GetWindowText(hWnd, sb, sb.Capacity);
        return sb.ToString();
    }
}