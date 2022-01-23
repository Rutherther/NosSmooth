//
//  PetManagerList.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.LocalBinding.Errors;
using NosSmooth.LocalBinding.Extensions;
using NosSmooth.LocalBinding.Options;
using Reloaded.Memory.Sources;
using Remora.Results;

namespace NosSmooth.LocalBinding.Structs;

/// <summary>
/// NosTale list of <see cref="PetManager"/>.
/// </summary>
public class PetManagerList
{
    /// <summary>
    /// Create <see cref="PlayerManager"/> instance.
    /// </summary>
    /// <param name="nosBrowser">The NosTale process browser.</param>
    /// <param name="options">The options.</param>
    /// <returns>The player manager or an error.</returns>
    public static Result<PetManagerList> Create(ExternalNosBrowser nosBrowser, PetManagerOptions options)
    {
        var characterObjectAddress = nosBrowser.Scanner.CompiledFindPattern(options.PetManagerListPattern);
        if (!characterObjectAddress.Found)
        {
            return new BindingNotFoundError(options.PetManagerListPattern, "PetManagerList");
        }

        if (nosBrowser.Process.MainModule is null)
        {
            return new NotFoundError("Cannot find the main module of the target process.");
        }

        int staticManagerAddress = (int)nosBrowser.Process.MainModule.BaseAddress + characterObjectAddress.Offset;
        return new PetManagerList(nosBrowser.Memory, staticManagerAddress, options.PetManagerListOffsets);
    }

    private readonly IMemory _memory;
    private readonly int _staticPetManagerListAddress;
    private readonly int[] _staticPetManagerOffsets;

    /// <summary>
    /// Initializes a new instance of the <see cref="PetManagerList"/> class.
    /// </summary>
    /// <param name="memory">The memory.</param>
    /// <param name="staticPetManagerListAddress">The static pet manager address.</param>
    /// <param name="staticPetManagerOffsets">The offsets to follow to the pet manager list address.</param>
    public PetManagerList(IMemory memory, int staticPetManagerListAddress, int[] staticPetManagerOffsets)
    {
        _memory = memory;
        _staticPetManagerListAddress = staticPetManagerListAddress;
        _staticPetManagerOffsets = staticPetManagerOffsets;
    }

    /// <summary>
    /// Gets the address of the pet manager.
    /// </summary>
    /// <returns>An address to the pet manager list.</returns>
    public IntPtr Address => _memory.FollowStaticAddressOffsets(_staticPetManagerListAddress, _staticPetManagerOffsets);

    /// <summary>
    /// Gets the length of the array.
    /// </summary>
    public int Length
    {
        get
        {
            _memory.SafeRead(Address + 0x08, out int length);
            return length;
        }
    }

    private IntPtr List
    {
        get
        {
            _memory.SafeRead(Address + 0x04, out int listPointer);
            return (IntPtr)listPointer;
        }
    }

    /// <summary>
    /// Get the first pet.
    /// </summary>
    /// <returns>First pet, if exists.</returns>
    public MapNpcObj? GetFirst()
    {
        if (Length == 0)
        {
            return null;
        }

        _memory.SafeRead(List, out int firstAddress);
        return new MapNpcObj(_memory, (IntPtr)firstAddress);
    }

    /// <summary>
    /// Get the second pet.
    /// </summary>
    /// <returns>Second pet, if exists.</returns>
    public MapNpcObj? GetSecond()
    {
        if (Length < 2)
        {
            return null;
        }

        _memory.SafeRead(List + 0x04, out int secondAddress);
        return new MapNpcObj(_memory, (IntPtr)secondAddress);
    }
}