//
//  DllMain.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace WalkCommands;

/// <summary>
/// Represents the dll entrypoint class.
/// </summary>
public class DllMain
{
    [DllImport("kernel32")]
#pragma warning disable SA1600
    public static extern bool AllocConsole();
#pragma warning restore SA1600

    /// <summary>
    /// Represents the dll entrypoint method.
    /// </summary>
    [DllExport]
    public static void Main()
    {
        AllocConsole();
        new Thread(() =>
        {
            try
            {
                new Startup().RunAsync().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }).Start();
    }
}