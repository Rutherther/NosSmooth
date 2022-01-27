//
//  MapBaseObj.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Reloaded.Memory.Sources;

namespace NosSmooth.LocalBinding.Structs;

/// <summary>
/// Base map object. Common for players, monsters, npcs.
/// </summary>
public class MapBaseObj : NostaleObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapBaseObj"/> class.
    /// </summary>
    public MapBaseObj()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MapBaseObj"/> class.
    /// </summary>
    /// <param name="memory">The memory.</param>
    /// <param name="mapObjPointer">The map object pointer.</param>
    public MapBaseObj(IMemory memory, IntPtr mapObjPointer)
        : base(memory, mapObjPointer)
    {
    }

    /// <summary>
    /// Gets the id of the entity.
    /// </summary>
    public long Id
    {
        get
        {
            Memory.SafeRead(Address + 0x08, out int id);
            return id;
        }
    }

    /// <summary>
    /// Gets the x coordinate of the entity.
    /// </summary>
    public ushort X
    {
        get
        {
            Memory.SafeRead(Address + 0x0C, out ushort x);
            return x;
        }
    }

    /// <summary>
    /// Gets the y coordinate of the entity.
    /// </summary>
    public ushort Y
    {
        get
        {
            Memory.SafeRead(Address + 0x0E, out ushort y);
            return y;
        }
    }
}