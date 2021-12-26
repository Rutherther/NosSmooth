//
//  DllMain.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace SimplePiiBot;

/// <summary>
/// The entrypoint class of the dll.
/// </summary>
public class DllMain
{
    [DllImport("kernel32")]
#pragma warning disable SA1600
    public static extern bool AllocConsole();
#pragma warning restore SA1600

    /// <summary>
    /// The entrypoint method of the dll.
    /// </summary>
    [DllExport]
    public static void Main()
    {
        AllocConsole();
        Task.Run(new PiiBot().RunAsync);
    }
}