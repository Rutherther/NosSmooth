//
//  LoadParams.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NosSmooth.Injector
{
    /// <summary>
    /// The parameters passed to the inject module.
    /// </summary>
    internal struct LoadParams
    {
        /// <summary>
        /// The full path of the library.
        /// </summary>
        public int LibraryPath;

        /// <summary>
        /// The full path of the library.
        /// </summary>
        public int RuntimeConfigPath;

        /// <summary>
        /// The full path to the type with the method marked as UnsafeCallersOnly.
        /// </summary>
        /// <remarks>
        /// Can be for example "LibraryNamespace.Type, LibraryNamespace".
        /// </remarks>
        public int TypePath;

        /// <summary>
        /// The name of the method to execute.
        /// </summary>
        public int MethodName;
    }
}
