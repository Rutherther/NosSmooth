//
//  PlayerManagerBinding.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using NosSmooth.LocalBinding.Errors;
using NosSmooth.LocalBinding.Options;
using NosSmooth.LocalBinding.Structs;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;
using Remora.Results;

namespace NosSmooth.LocalBinding.Objects;

/// <summary>
/// The nostale binding of a character.
/// </summary>
public class PlayerManagerBinding
{
    [Function
    (
        new[] { FunctionAttribute.Register.eax, FunctionAttribute.Register.edx, FunctionAttribute.Register.ecx },
        FunctionAttribute.Register.eax,
        FunctionAttribute.StackCleanup.Callee
    )]
    private delegate bool WalkDelegate(IntPtr playerManagerPtr, int position, short unknown0 = 0, int unknown1 = 1);

    [Function
    (
        new[] { FunctionAttribute.Register.eax, FunctionAttribute.Register.edx, FunctionAttribute.Register.ecx },
        FunctionAttribute.Register.eax,
        FunctionAttribute.StackCleanup.Callee
    )]
    private delegate bool FollowEntityDelegate
    (
        IntPtr playerManagerPtr,
        IntPtr entityPtr,
        int unknown1 = 0,
        int unknown2 = 1
    );

    [Function
    (
        new[] { FunctionAttribute.Register.eax, FunctionAttribute.Register.edx },
        FunctionAttribute.Register.eax,
        FunctionAttribute.StackCleanup.Callee
    )]
    private delegate void UnfollowEntityDelegate(IntPtr playerManagerPtr, int unknown = 0);

    /// <summary>
    /// Create the network binding with finding the network object and functions.
    /// </summary>
    /// <param name="bindingManager">The binding manager.</param>
    /// <param name="playerManager">The player manager.</param>
    /// <param name="options">The options for the binding.</param>
    /// <returns>A network binding or an error.</returns>
    public static Result<PlayerManagerBinding> Create(NosBindingManager bindingManager, PlayerManager playerManager, CharacterBindingOptions options)
    {
        var process = Process.GetCurrentProcess();

        var walkFunctionAddress = bindingManager.Scanner.CompiledFindPattern(options.WalkFunctionPattern);
        if (!walkFunctionAddress.Found)
        {
            return new BindingNotFoundError(options.WalkFunctionPattern, "CharacterBinding.Walk");
        }

        var followEntityAddress = bindingManager.Scanner.CompiledFindPattern(options.FollowEntityPattern);
        if (!followEntityAddress.Found)
        {
            return new BindingNotFoundError(options.FollowEntityPattern, "CharacterBinding.FollowEntity");
        }

        var unfollowEntityAddress = bindingManager.Scanner.CompiledFindPattern(options.UnfollowEntityPattern);
        if (!unfollowEntityAddress.Found)
        {
            return new BindingNotFoundError(options.UnfollowEntityPattern, "CharacterBinding.UnfollowEntity");
        }

        var walkFunction = bindingManager.Hooks.CreateFunction<WalkDelegate>
            (walkFunctionAddress.Offset + (int)process.MainModule!.BaseAddress);
        var walkWrapper = walkFunction.GetWrapper();

        var followEntityFunction = bindingManager.Hooks.CreateFunction<FollowEntityDelegate>
            (followEntityAddress.Offset + (int)process.MainModule!.BaseAddress);
        var followEntityWrapper = followEntityFunction.GetWrapper();

        var unfollowEntityFunction = bindingManager.Hooks.CreateFunction<UnfollowEntityDelegate>
            (unfollowEntityAddress.Offset + (int)process.MainModule!.BaseAddress);
        var unfollowEntityWrapper = unfollowEntityFunction.GetWrapper();

        var binding = new PlayerManagerBinding
        (
            bindingManager,
            playerManager,
            walkWrapper,
            followEntityWrapper,
            unfollowEntityWrapper
        );

        if (options.HookWalk)
        {
            binding._walkHook = walkFunction
                .Hook(binding.WalkDetour);
            binding._originalWalk = binding._walkHook.OriginalFunction;
        }

        if (options.HookFollowEntity)
        {
            binding._followHook = followEntityFunction.Hook(binding.FollowEntityDetour);
            binding._originalFollowEntity = binding._followHook.OriginalFunction;
        }

        if (options.HookUnfollowEntity)
        {
            binding._unfollowHook = unfollowEntityFunction.Hook(binding.UnfollowEntityDetour);
            binding._originalUnfollowEntity = binding._unfollowHook.OriginalFunction;
        }

        binding._walkHook?.Activate();
        binding._followHook?.Activate();
        binding._unfollowHook?.Activate();
        return binding;
    }

    private readonly NosBindingManager _bindingManager;

    private IHook<WalkDelegate>? _walkHook;
    private IHook<FollowEntityDelegate>? _followHook;
    private IHook<UnfollowEntityDelegate>? _unfollowHook;

    private FollowEntityDelegate _originalFollowEntity;
    private UnfollowEntityDelegate _originalUnfollowEntity;
    private WalkDelegate _originalWalk;

    private PlayerManagerBinding
    (
        NosBindingManager bindingManager,
        PlayerManager playerManager,
        WalkDelegate originalWalk,
        FollowEntityDelegate originalFollowEntity,
        UnfollowEntityDelegate originalUnfollowEntity
    )
    {
        PlayerManager = playerManager;
        _bindingManager = bindingManager;
        _originalWalk = originalWalk;
        _originalFollowEntity = originalFollowEntity;
        _originalUnfollowEntity = originalUnfollowEntity;
    }

    /// <summary>
    /// Gets the player manager.
    /// </summary>
    public PlayerManager PlayerManager { get; }

    /// <summary>
    /// Event that is called when walk was called by NosTale.
    /// </summary>
    /// <remarks>
    /// The walk must be hooked for this event to be called.
    /// </remarks>
    public event Func<ushort, ushort, bool>? WalkCall;

    /// <summary>
    /// Event that is called when entity follow or unfollow was called.
    /// </summary>
    /// <remarks>
    /// The follow/unfollow entity must be hooked for this event to be called.
    /// </remarks>
    public event Func<MapBaseObj?, bool>? FollowEntityCall;

    /// <summary>
    /// Disable all the hooks that are currently enabled.
    /// </summary>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result DisableHooks()
    {
        _walkHook?.Disable();
        return Result.FromSuccess();
    }

    /// <summary>
    /// Walk to the given position.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result<bool> Walk(ushort x, ushort y)
    {
        int param = (y << 16) | x;
        try
        {
            return _originalWalk(PlayerManager.Address, param);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    private bool WalkDetour(IntPtr characterObject, int position, short unknown0, int unknown1)
    {
        var result = WalkCall?.Invoke((ushort)(position & 0xFFFF), (ushort)((position >> 16) & 0xFFFF));
        if (result ?? true)
        {
            return _originalWalk(characterObject, position, unknown0, unknown1);
        }

        return false;
    }

    /// <summary>
    /// Follow the entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result FollowEntity(MapBaseObj? entity)
        => FollowEntity(entity?.Address ?? IntPtr.Zero);

    /// <summary>
    /// Follow the entity.
    /// </summary>
    /// <param name="entityAddress">The entity address.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result FollowEntity(IntPtr entityAddress)
    {
        try
        {
            _originalFollowEntity(PlayerManager.Address, entityAddress);
        }
        catch (Exception e)
        {
            return e;
        }

        return Result.FromSuccess();
    }

    /// <summary>
    /// Stop following entity.
    /// </summary>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result UnfollowEntity()
    {
        try
        {
            _originalUnfollowEntity(PlayerManager.Address);
        }
        catch (Exception e)
        {
            return e;
        }

        return Result.FromSuccess();
    }

    private bool FollowEntityDetour
    (
        IntPtr playerManagerPtr,
        IntPtr entityPtr,
        int unknown1,
        int unknown2
    )
    {
        var result = FollowEntityCall?.Invoke(new MapBaseObj(_bindingManager.Memory, entityPtr));
        if (result ?? true)
        {
            return _originalFollowEntity(playerManagerPtr, entityPtr, unknown1, unknown2);
        }

        return false;
    }

    private void UnfollowEntityDetour(IntPtr playerManagerPtr, int unknown)
    {
        var result = FollowEntityCall?.Invoke(null);
        if (result ?? true)
        {
            _originalUnfollowEntity(playerManagerPtr, unknown);
        }
    }
}