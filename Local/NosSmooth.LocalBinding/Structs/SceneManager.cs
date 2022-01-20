//
//  SceneManager.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Reloaded.Memory.Sources;

namespace NosSmooth.LocalBinding.Structs;

/// <summary>
/// Represents nostale scene manager struct.
/// </summary>
public class SceneManager
{
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

    private IntPtr ReadPtr(IntPtr ptr)
    {
        _memory.Read(ptr, out IntPtr read);
        return read;
    }
}