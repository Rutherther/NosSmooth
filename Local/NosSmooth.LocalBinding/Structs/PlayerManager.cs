//
//  PlayerManager.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Options;
using NosSmooth.LocalBinding.Errors;
using NosSmooth.LocalBinding.Options;
using Reloaded.Memory.Sources;
using Remora.Results;

namespace NosSmooth.LocalBinding.Structs;

/// <summary>
/// NosTale player manager.
/// </summary>
public class PlayerManager
{
    /// <summary>
    /// Create <see cref="PlayerManager"/> instance.
    /// </summary>
    /// <param name="nosBrowser">The NosTale process browser.</param>
    /// <param name="options">The options.</param>
    /// <returns>The player manager or an error.</returns>
    public static Result<PlayerManager> Create(ExternalNosBrowser nosBrowser, CharacterBindingOptions options)
    {
        var characterObjectAddress = nosBrowser.Scanner.CompiledFindPattern(options.CharacterObjectPattern);
        if (!characterObjectAddress.Found)
        {
            return new BindingNotFoundError(options.CharacterObjectPattern, "PlayerManager");
        }

        if (nosBrowser.Process.MainModule is null)
        {
            return new NotFoundError("Cannot find the main module of the target process.");
        }

        var ptrAddress = nosBrowser.Process.MainModule.BaseAddress + characterObjectAddress.Offset + 0x06;
        nosBrowser.Memory.SafeRead(ptrAddress, out int address);
        nosBrowser.Memory.SafeRead((IntPtr)address, out address);
        return new PlayerManager(nosBrowser.Memory, (IntPtr)address);
    }

    private readonly IMemory _memory;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerManager"/> class.
    /// </summary>
    /// <param name="memory">The memory.</param>
    /// <param name="playerManager">The pointer to the beginning of the player manager structure.</param>
    public PlayerManager(IMemory memory, IntPtr playerManager)
    {
        _memory = memory;
        Address = playerManager;
    }

    /// <summary>
    /// Gets the address to the player manager.
    /// </summary>
    public IntPtr Address { get; }

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

    /// <summary>
    /// Gets the player object.
    /// </summary>
    public MapPlayerObj Player
    {
        get
        {
            _memory.SafeRead(Address + 0x20, out int playerAddress);
            return new MapPlayerObj(_memory, (IntPtr)playerAddress);
        }
    }

    /// <summary>
    /// Gets the player id.
    /// </summary>
    public int PlayerId
    {
        get
        {
            _memory.SafeRead(Address + 0x24, out int playerId);
            return playerId;
        }
    }
}