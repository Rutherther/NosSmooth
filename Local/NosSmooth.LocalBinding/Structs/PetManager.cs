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
    private readonly IMemory _memory;

    /// <summary>
    /// Initializes a new instance of the <see cref="PetManager"/> class.
    /// </summary>
    /// <param name="memory">The memory.</param>
    /// <param name="petManagerAddress">The pet manager address.</param>
    public PetManager(IMemory memory, IntPtr petManagerAddress)
        : base(memory)
    {
        _memory = memory;
        Address = petManagerAddress;
    }

    /// <summary>
    /// Gets the address of the pet manager.
    /// </summary>
    public override IntPtr Address { get; }

    /// <summary>
    /// Gets the player object.
    /// </summary>
    public MapNpcObj Pet
    {
        get
        {
            _memory.SafeRead(Address + 0x7C, out int playerAddress);
            return new MapNpcObj(_memory, (IntPtr)playerAddress);
        }
    }
}