//
//  PetManager.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.LocalBinding.Errors;
using NosSmooth.LocalBinding.Options;
using Reloaded.Memory.Sources;
using Remora.Results;

namespace NosSmooth.LocalBinding.Structs;

/// <summary>
/// NosTale pet manager.
/// </summary>
public class PetManager : ControlManager
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PetManager"/> class.
    /// </summary>
    public PetManager()
        : base(null!, IntPtr.Zero)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PetManager"/> class.
    /// </summary>
    /// <param name="memory">The memory.</param>
    /// <param name="petManagerAddress">The pet manager address.</param>
    public PetManager(IMemory memory, IntPtr petManagerAddress)
        : base(memory, petManagerAddress)
    {
    }

    /// <summary>
    /// Gets the player object.
    /// </summary>
    public MapNpcObj Pet
    {
        get
        {
            Memory.SafeRead(Address + 0x7C, out int playerAddress);
            return new MapNpcObj(Memory, (IntPtr)playerAddress);
        }
    }
}