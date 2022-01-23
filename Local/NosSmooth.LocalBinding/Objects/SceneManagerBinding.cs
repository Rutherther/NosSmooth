//
//  SceneManagerBinding.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using NosSmooth.LocalBinding.Errors;
using NosSmooth.LocalBinding.Options;
using NosSmooth.LocalBinding.Structs;
using Reloaded.Hooks;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory.Buffers.Internal.Kernel32;
using Remora.Results;

namespace NosSmooth.LocalBinding.Objects;

/// <summary>
/// The nostale binding of a scene manager.
/// </summary>
/// <remarks>
/// The scene manager holds addresses to entities, mouse position, ....
/// </remarks>
public class SceneManagerBinding
{
    [Function
    (
        new[] { FunctionAttribute.Register.eax, FunctionAttribute.Register.edx },
        FunctionAttribute.Register.ecx,
        FunctionAttribute.StackCleanup.Callee
    )]
    private delegate void FocusEntityDelegate(IntPtr outputStringPtr, IntPtr entityPtr);

    /// <summary>
    /// Create the scene manager binding.
    /// </summary>
    /// <param name="bindingManager">The binding manager.</param>
    /// <param name="sceneManager">The scene manager.</param>
    /// <param name="bindingOptions">The options for the binding.</param>
    /// <returns>A network binding or an error.</returns>
    public static Result<SceneManagerBinding> Create
        (NosBindingManager bindingManager, SceneManager sceneManager, SceneManagerBindingOptions bindingOptions)
    {
        var process = Process.GetCurrentProcess();

        var focusEntityAddress = bindingManager.Scanner.CompiledFindPattern(bindingOptions.FocusEntityPattern);
        if (!focusEntityAddress.Found)
        {
            return new BindingNotFoundError(bindingOptions.FocusEntityPattern, "SceneManagerBinding.FocusEntity");
        }

        var focusEntityFunction = bindingManager.Hooks.CreateFunction<FocusEntityDelegate>
            (focusEntityAddress.Offset + (int)process.MainModule!.BaseAddress + 0x04);
        var focusEntityWrapper = focusEntityFunction.GetWrapper();

        var binding = new SceneManagerBinding
        (
            bindingManager,
            sceneManager,
            focusEntityWrapper
        );

        if (bindingOptions.HookFocusEntity)
        {
            binding._focusHook = focusEntityFunction.Hook(binding.FocusEntityDetour);
            binding._originalFocusEntity = binding._focusHook.OriginalFunction;
        }

        binding._focusHook?.Activate();
        return binding;
    }

    private readonly NosBindingManager _bindingManager;
    private FocusEntityDelegate _originalFocusEntity;

    private IHook<FocusEntityDelegate>? _focusHook;

    private SceneManagerBinding
    (
        NosBindingManager bindingManager,
        SceneManager sceneManager,
        FocusEntityDelegate originalFocusEntity
    )
    {
        _originalFocusEntity = originalFocusEntity;
        _bindingManager = bindingManager;
        SceneManager = sceneManager;
    }

    /// <summary>
    /// Event that is called when entity focus was called by NosTale.
    /// </summary>
    /// <remarks>
    /// The focus entity must be hooked for this event to be called.
    /// </remarks>
    public event Func<MapBaseObj?, bool>? EntityFocus;

    /// <summary>
    /// Gets the scene manager object.
    /// </summary>
    public SceneManager SceneManager { get; }

    /// <summary>
    /// Focus the entity with the given id.
    /// </summary>
    /// <param name="id">The id of the entity.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result FocusEntity(int id)
    {
        var entityResult = FindEntity(id);
        if (!entityResult.IsSuccess)
        {
            return Result.FromError(entityResult);
        }

        return FocusEntity(entityResult.Entity);
    }

    /// <summary>
    /// Focus the entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result FocusEntity(MapBaseObj? entity)
        => FocusEntity(entity?.Address ?? IntPtr.Zero);

    /// <summary>
    /// Focus the entity.
    /// </summary>
    /// <param name="entityAddress">The entity address.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result FocusEntity(IntPtr entityAddress)
    {
        try
        {
            _originalFocusEntity(IntPtr.Zero, entityAddress);
        }
        catch (Exception e)
        {
            return e;
        }

        return Result.FromSuccess();
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

        var manager = SceneManager;

        var item = manager.ItemList.FirstOrDefault(x => x.Id == id);
        if (item is not null)
        {
            return item;
        }

        var monster = manager.MonsterList.FirstOrDefault(x => x.Id == id);
        if (monster is not null)
        {
            return monster;
        }

        var npc = manager.NpcList.FirstOrDefault(x => x.Id == id);
        if (npc is not null)
        {
            return npc;
        }

        var player = manager.PlayerList.FirstOrDefault(x => x.Id == id);
        if (player is not null)
        {
            return player;
        }

        return new NotFoundError($"Could not find entity with id {id}");
    }

    private void FocusEntityDetour(IntPtr outputStringPtr, IntPtr entityId)
    {
        MapBaseObj? obj = null;
        if (entityId != IntPtr.Zero)
        {
            obj = new MapBaseObj(_bindingManager.Memory, entityId);
        }

        var result = EntityFocus?.Invoke(obj);
        if (result ?? true)
        {
            _originalFocusEntity(outputStringPtr, entityId);
        }
    }
}