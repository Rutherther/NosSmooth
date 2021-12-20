//
//  DllMain.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using NosSmooth.Core.Client;
using NosSmooth.Core.Extensions;
using NosSmooth.LocalClient.Extensions;

namespace Test
{
    /// <summary>
    /// Entry point of the dll.
    /// </summary>
    public class DllMain
    {
        /// <summary>
        /// Create console.
        /// </summary>
        public static void CreateConsole()
        {
            AllocConsole();
        }

        [DllImport(
            "kernel32.dll",
            EntryPoint = "GetStdHandle",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport(
            "kernel32.dll",
            EntryPoint = "AllocConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();

        /// <summary>
        /// The entrypoint method.
        /// </summary>
        /// <param name="moduleHandle">The handle of the dll.</param>
        /// <returns>The error code.</returns>
        [DllExport]
        public static int Main(IntPtr moduleHandle)
        {
            CreateConsole();

            Task.Run(Start);
            return 0;
        }

        private static Task Start()
        {
            return new Test().Start();
        }
    }
}