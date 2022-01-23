//
//  ControlManager.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Reloaded.Memory.Sources;

namespace NosSmooth.LocalBinding.Structs;

/// <summary>
/// Base for player and pet managers.
/// </summary>
public abstract class ControlManager
{
    private readonly IMemory _memory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlManager"/> class.
    /// </summary>
    /// <param name="memory">The memory.</param>
    public ControlManager(IMemory memory)
    {
        _memory = memory;
    }

    /// <summary>
    /// Gets the address of the manager.
    /// </summary>
    public abstract IntPtr Address { get; }

    /// <summary>
    /// Gets the current player position x coordinate.
    /// </summary>
    public int X
    {
        get
        {
            _memory.SafeRead(Address + 0x4, out short x);
            return x;
        }
    }

    /// <summary>
    /// Gets the current player position x coordinate.
    /// </summary>
    public int Y
    {
        get
        {
            _memory.SafeRead(Address + 0x6, out short y);
            return y;
        }
    }

    /// <summary>
    /// Gets the target x coordinate the player is moving to.
    /// </summary>
    public int TargetX
    {
        get
        {
            _memory.SafeRead(Address + 0x8, out short targetX);
            return targetX;
        }
    }

    /// <summary>
    /// Gets the target y coordinate the player is moving to.
    /// </summary>
    public int TargetY
    {
        get
        {
            _memory.SafeRead(Address + 0xA, out short targetX);
            return targetX;
        }
    }
}