//
//  DllMain.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace SimpleChat;

/// <summary>
/// The main entrypoint class of the dll.
/// </summary>
public class DllMain
{
    [DllImport("kernel32")]
#pragma warning disable SA1600
    public static extern bool AllocConsole();
#pragma warning restore SA1600

    /// <summary>
    /// The main entrypoint method of the dll.
    /// </summary>
    /// <param name="handle">The handle of the module.</param>
    [DllExport]
    public static void Main(IntPtr handle)
    {
        AllocConsole();
        Console.WriteLine("Hello from SimpleChat DllMain entry point.");

        new Thread(() => new SimpleChat().RunAsync().GetAwaiter().GetResult()).Start();
    }
}