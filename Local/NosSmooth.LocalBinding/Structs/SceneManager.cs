//
//  SceneManager.cs
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
/// Represents nostale scene manager struct.
/// </summary>
public class SceneManager
{
    /// <summary>
    /// Create <see cref="PlayerManager"/> instance.
    /// </summary>
    /// <param name="nosBrowserManager">The NosTale process browser.</param>
    /// <param name="options">The options.</param>
    /// <returns>The player manager or an error.</returns>
    public static Result<SceneManager> Create(NosBrowserManager nosBrowserManager, SceneManagerOptions options)
    {
        var characterObjectAddress = nosBrowserManager.Scanner.CompiledFindPattern(options.SceneManagerObjectPattern);
        if (!characterObjectAddress.Found)
        {
            return new BindingNotFoundError(options.SceneManagerObjectPattern, "SceneManager");
        }

        if (nosBrowserManager.Process.MainModule is null)
        {
            return new NotFoundError("Cannot find the main module of the target process.");
        }

        int staticManagerAddress = (int)nosBrowserManager.Process.MainModule.BaseAddress + characterObjectAddress.Offset;
        return new SceneManager(nosBrowserManager.Memory, staticManagerAddress, options.SceneManagerOffsets);
    }

    private readonly int[] _sceneManagerOffsets;
    private readonly IMemory _memory;
    private readonly int _staticSceneManagerAddress;

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneManager"/> class.
    /// </summary>
    /// <param name="memory">The memory.</param>
    /// <param name="staticSceneManagerAddress">The pointer to the scene manager.</param>
    /// <param name="sceneManagerOffsets">The offsets from the static scene manager address.</param>
    public SceneManager(IMemory memory, int staticSceneManagerAddress, int[] sceneManagerOffsets)
    {
        _memory = memory;
        _staticSceneManagerAddress = staticSceneManagerAddress;
        _sceneManagerOffsets = sceneManagerOffsets;
    }

    /// <summary>
    /// Gets the address of the scene manager.
    /// </summary>
    public IntPtr Address => _memory.FollowStaticAddressOffsets(_staticSceneManagerAddress, _sceneManagerOffsets);

    /// <summary>
    /// Gets the player list.
    /// </summary>
    public NostaleList<MapBaseObj> PlayerList => new NostaleList<MapBaseObj>(_memory, ReadPtr(Address + 0xC));

    /// <summary>
    /// Gets the monster list.
    /// </summary>
    public NostaleList<MapBaseObj> MonsterList => new NostaleList<MapBaseObj>(_memory, ReadPtr(Address + 0x10));

    /// <summary>
    /// Gets the npc list.
    /// </summary>
    public NostaleList<MapBaseObj> NpcList => new NostaleList<MapBaseObj>(_memory, ReadPtr(Address + 0x14));

    /// <summary>
    /// Gets the item list.
    /// </summary>
    public NostaleList<MapBaseObj> ItemList => new NostaleList<MapBaseObj>(_memory, ReadPtr(Address + 0x18));

    /// <summary>
    /// Gets the entity that is currently being followed by the player.
    /// </summary>
    public MapBaseObj? FollowEntity
    {
        get
        {
            var ptr = ReadPtr(Address + 0x48);
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            return new MapBaseObj(_memory, ptr);
        }
    }

    /// <summary>
    /// Gets the lock on target marked address.
    /// </summary>
    public IntPtr LockOnTargetMarkedAddress
    {
        get
        {
            var ptr = ReadPtr(Address + 0x1C);
            ptr = ReadPtr(ptr + 0x04);
            ptr = ReadPtr(ptr + 0x00);
            return ptr;
        }
    }

    private IntPtr ReadPtr(IntPtr ptr)
    {
        _memory.Read(ptr, out int read);
        return (IntPtr)read;
    }

    /// <summary>
    /// Find the given entity address.
    /// </summary>
    /// <param name="id">The id of the entity.</param>
    /// <returns>The pointer to the entity or an error.</returns>
    public Result<MapBaseObj?> FindEntity(int id)
    {
        if (id == 0)
        {
            return Result<MapBaseObj?>.FromSuccess(null);
        }

        var item = ItemList.FirstOrDefault(x => x.Id == id);
        if (item is not null)
        {
            return item;
        }

        var monster = MonsterList.FirstOrDefault(x => x.Id == id);
        if (monster is not null)
        {
            return monster;
        }

        var npc = NpcList.FirstOrDefault(x => x.Id == id);
        if (npc is not null)
        {
            return npc;
        }

        var player = PlayerList.FirstOrDefault(x => x.Id == id);
        if (player is not null)
        {
            return player;
        }

        return new NotFoundError($"Could not find entity with id {id}");
    }
}