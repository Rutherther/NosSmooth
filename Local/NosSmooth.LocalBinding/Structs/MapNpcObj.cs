//
//  MapNpcObj.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Reloaded.Memory.Sources;

namespace NosSmooth.LocalBinding.Structs;

/// <summary>
/// Npc NosTale object.
/// </summary>
public class MapNpcObj : MapBaseObj
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapNpcObj"/> class.
    /// </summary>
    /// <param name="memory">The memory.</param>
    /// <param name="mapObjPointer">The pointer to the object.</param>
    public MapNpcObj(IMemory memory, IntPtr mapObjPointer)
        : base(memory, mapObjPointer)
    {
    }
}