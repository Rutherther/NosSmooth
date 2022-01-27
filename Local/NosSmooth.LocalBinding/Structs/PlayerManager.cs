//
//  PlayerManager.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Options;
using NosSmooth.LocalBinding.Errors;
using NosSmooth.LocalBinding.Extensions;
using NosSmooth.LocalBinding.Options;
using Reloaded.Memory.Sources;
using Remora.Results;

namespace NosSmooth.LocalBinding.Structs;

/// <summary>
/// NosTale player manager.
/// </summary>
public class PlayerManager : ControlManager
{
    /// <summary>
    /// Create <see cref="PlayerManager"/> instance.
    /// </summary>
    /// <param name="nosBrowserManager">The NosTale process browser.</param>
    /// <param name="options">The options.</param>
    /// <returns>The player manager or an error.</returns>
    public static Result<PlayerManager> Create(NosBrowserManager nosBrowserManager, PlayerManagerOptions options)
    {
        var characterObjectAddress = nosBrowserManager.Scanner.CompiledFindPattern(options.PlayerManagerPattern);
        if (!characterObjectAddress.Found)
        {
            return new BindingNotFoundError(options.PlayerManagerPattern, "PlayerManager");
        }

        if (nosBrowserManager.Process.MainModule is null)
        {
            return new NotFoundError("Cannot find the main module of the target process.");
        }

        var staticAddress = (int)nosBrowserManager.Process.MainModule.BaseAddress + characterObjectAddress.Offset;
        return new PlayerManager(nosBrowserManager.Memory, staticAddress, options.PlayerManagerOffsets);
    }

    private readonly IMemory _memory;
    private readonly int _staticPlayerManagerAddress;
    private readonly int[] _playerManagerOffsets;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerManager"/> class.
    /// </summary>
    /// <param name="memory">The memory.</param>
    /// <param name="staticPlayerManagerAddress">The pointer to the beginning of the player manager structure.</param>
    /// <param name="playerManagerOffsets">The offsets to get the player manager address from the static one.</param>
    public PlayerManager(IMemory memory, int staticPlayerManagerAddress, int[] playerManagerOffsets)
        : base(memory, IntPtr.Zero)
    {
        _memory = memory;
        _staticPlayerManagerAddress = staticPlayerManagerAddress;
        _playerManagerOffsets = playerManagerOffsets;
    }

    /// <summary>
    /// Gets the address to the player manager.
    /// </summary>
    public override IntPtr Address => _memory.FollowStaticAddressOffsets
        (_staticPlayerManagerAddress, _playerManagerOffsets);

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

    /// <inheritdoc />
    public override MapBaseObj Entity => Player;
}