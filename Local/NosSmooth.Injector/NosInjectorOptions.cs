//
//  NosInjectorOptions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace NosSmooth.Injector
{
    /// <summary>
    /// Options for NosInjector.
    /// </summary>
    public class NosInjectorOptions : IOptions<NosInjectorOptions>
    {
        /// <summary>
        /// Gets or sets the path to the nos smooth inject dll.
        /// </summary>
        /// <remarks>
        /// If not absolute path, then relative path from the current executing process is assumed.
        /// </remarks>
        public string NosSmoothInjectPath { get; set; } = "NosSmooth.Inject.dll";

        /// <inheritdoc/>
        public NosInjectorOptions Value => this;
    }
}
