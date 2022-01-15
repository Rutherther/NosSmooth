//
//  CharacterBinding.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using NosSmooth.LocalBinding.Errors;
using NosSmooth.LocalBinding.Options;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;
using Remora.Results;

namespace NosSmooth.LocalBinding.Objects;

/// <summary>
/// The nostale binding of a character.
/// </summary>
public class CharacterBinding
{
        [Function
    (
        new[] { FunctionAttribute.Register.eax, FunctionAttribute.Register.edx },
        FunctionAttribute.Register.eax,
        FunctionAttribute.StackCleanup.Callee
    )]
    private delegate void WalkDelegate(IntPtr characterObject, int position, int unknown = 1);

    /// <summary>
    /// Create the network binding with finding the network object and functions.
    /// </summary>
    /// <param name="bindingManager">The binding manager.</param>
    /// <param name="options">The options for the binding.</param>
    /// <returns>A network binding or an error.</returns>
    public static Result<CharacterBinding> Create(NosBindingManager bindingManager, CharacterBindingOptions options)
    {
        var process = Process.GetCurrentProcess();
        var characterObjectAddress = bindingManager.Scanner.CompiledFindPattern(options.CharacterObjectPattern);
        if (!characterObjectAddress.Found)
        {
            return new BindingNotFoundError(options.CharacterObjectPattern, "CharacterBinding");
        }

        var walkFunctionAddress = bindingManager.Scanner.CompiledFindPattern(options.WalkFunctionPattern);
        if (!walkFunctionAddress.Found)
        {
            return new BindingNotFoundError(options.WalkFunctionPattern, "CharacterBinding.Walk");
        }

        var walkFunction = bindingManager.Hooks.CreateFunction<WalkDelegate>
            (walkFunctionAddress.Offset + (int)process.MainModule!.BaseAddress);
        var walkWrapper = walkFunction.GetWrapper();

        var binding = new CharacterBinding
        (
            bindingManager,
            (IntPtr)(characterObjectAddress.Offset + (int)process.MainModule!.BaseAddress + 0x06),
            walkWrapper
        );

        if (options.HookWalk)
        {
            binding._walkHook = walkFunction
                .Hook(binding.WalkDetour);
            binding._originalWalk = binding._walkHook.OriginalFunction;
        }

        binding._walkHook?.Activate();
        return binding;
    }

    private readonly NosBindingManager _bindingManager;
    private readonly IntPtr _characterAddress;
    private IHook<WalkDelegate>? _walkHook;
    private WalkDelegate _originalWalk;

    private CharacterBinding
    (
        NosBindingManager bindingManager,
        IntPtr characterAddress,
        WalkDelegate originalWalk
    )
    {
        _bindingManager = bindingManager;
        _characterAddress = characterAddress;
        _originalWalk = originalWalk;
    }

    /// <summary>
    /// Event that is called when walk was called by NosTale.
    /// </summary>
    /// <remarks>
    /// The walk must be hooked for this event to be called.
    /// </remarks>
    public event Func<ushort, ushort, bool>? WalkCall;

    /// <summary>
    /// Disable all the hooks that are currently enabled.
    /// </summary>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result DisableHooks()
    {
        _walkHook?.Disable();
        return Result.FromSuccess();
    }

    private IntPtr GetCharacterAddress()
    {
        IntPtr characterAddress = _characterAddress;
        _bindingManager.Memory.Read(characterAddress, out characterAddress);
        _bindingManager.Memory.Read(characterAddress, out characterAddress);

        return characterAddress;
    }

    /// <summary>
    /// Walk to the given position.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <returns>A result that may or may not have succeeded.</returns>
    public Result Walk(ushort x, ushort y)
    {
        int param = (y << 16) | x;
        _originalWalk(GetCharacterAddress(), param);
        return Result.FromSuccess();
    }

    private void WalkDetour(IntPtr characterObject, int position, int unknown)
    {
        var result = WalkCall?.Invoke((ushort)(position & 0xFFFF), (ushort)((position >> 16) & 0xFFFF));
        if (result ?? true)
        {
            _originalWalk(characterObject, position, unknown);
        }
    }
}