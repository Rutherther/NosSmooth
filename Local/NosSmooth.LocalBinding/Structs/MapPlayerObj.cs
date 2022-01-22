//
//  MapPlayerObj.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using System.Text;
using Reloaded.Memory.Sources;

namespace NosSmooth.LocalBinding.Structs;

/// <summary>
/// NosTale Player object.
/// </summary>
public class MapPlayerObj : MapBaseObj
{
    private readonly IMemory _memory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapPlayerObj"/> class.
    /// </summary>
    /// <param name="memory">The memory.</param>
    /// <param name="mapObjPointer">The player object pointer.</param>
    public MapPlayerObj(IMemory memory, IntPtr mapObjPointer)
        : base(memory, mapObjPointer)
    {
        _memory = memory;
    }

    /// <summary>
    /// Gets the name of the player.
    /// </summary>
    public string? Name
    {
        get
        {
            _memory.SafeRead(Address + 0x1EC, out int nameAddress);
            _memory.SafeRead((IntPtr)nameAddress - 4, out int nameLength);
            byte[] data = new byte[nameLength];
            _memory.SafeReadRaw((IntPtr)nameAddress, out data, nameLength);
            return Encoding.ASCII.GetString(data);
        }
    }
}