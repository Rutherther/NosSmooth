//
//  SceneManager.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.LocalBinding.Errors;
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
    /// <param name="nosBrowser">The NosTale process browser.</param>
    /// <param name="options">The options.</param>
    /// <returns>The player manager or an error.</returns>
    public static Result<SceneManager> Create(ExternalNosBrowser nosBrowser, SceneManagerBindingOptions options)
    {
        var characterObjectAddress = nosBrowser.Scanner.CompiledFindPattern(options.SceneManagerObjectPattern);
        if (!characterObjectAddress.Found)
        {
            return new BindingNotFoundError(options.SceneManagerObjectPattern, "SceneManager");
        }

        if (nosBrowser.Process.MainModule is null)
        {
            return new NotFoundError("Cannot find the main module of the target process.");
        }

        var ptrAddress = nosBrowser.Process.MainModule.BaseAddress + characterObjectAddress.Offset;
        nosBrowser.Memory.SafeRead(ptrAddress, out ptrAddress);
        return new SceneManager(nosBrowser.Memory, ptrAddress);
    }

    private readonly IMemory _memory;
    private readonly IntPtr _sceneManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneManager"/> class.
    /// </summary>
    /// <param name="memory">The memory.</param>
    /// <param name="sceneManager">The pointer to the scene manager.</param>
    public SceneManager(IMemory memory, IntPtr sceneManager)
    {
        _memory = memory;
        _sceneManager = sceneManager;
    }

    /// <summary>
    /// Gets the player list.
    /// </summary>
    public MapObjBaseList PlayerList => new MapObjBaseList(_memory, ReadPtr(_sceneManager + 0xC));

    /// <summary>
    /// Gets the monster list.
    /// </summary>
    public MapObjBaseList MonsterList => new MapObjBaseList(_memory, ReadPtr(_sceneManager + 0x10));

    /// <summary>
    /// Gets the npc list.
    /// </summary>
    public MapObjBaseList NpcList => new MapObjBaseList(_memory, ReadPtr(_sceneManager + 0x14));

    /// <summary>
    /// Gets the item list.
    /// </summary>
    public MapObjBaseList ItemList => new MapObjBaseList(_memory, ReadPtr(_sceneManager + 0x18));

    /// <summary>
    /// Gets the entity that is currently being followed by the player.
    /// </summary>
    public MapBaseObj? FollowEntity
    {
        get
        {
            var ptr = ReadPtr(_sceneManager + 0x48);
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            return new MapBaseObj(_memory, ptr);
        }
    }

    private IntPtr ReadPtr(IntPtr ptr)
    {
        _memory.Read(ptr, out IntPtr read);
        return read;
    }
}