//
//  UnitManagerBinding.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using NosSmooth.LocalBinding.Errors;
using NosSmooth.LocalBinding.Extensions;
using NosSmooth.LocalBinding.Options;
using NosSmooth.LocalBinding.Structs;
using Reloaded.Hooks;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory.Buffers.Internal.Kernel32;
using Remora.Results;
using SharpDisasm.Udis86;

namespace NosSmooth.LocalBinding.Objects;

/// <summary>
/// The nostale binding of a scene manager.
/// </summary>
/// <remarks>
/// The scene manager holds addresses to entities, mouse position, ....
/// </remarks>
public class UnitManagerBinding
{
    [Function
    (
        new[] { FunctionAttribute.Register.eax, FunctionAttribute.Register.edx },
        FunctionAttribute.Register.eax,
        FunctionAttribute.StackCleanup.Callee
    )]
    private delegate int FocusEntityDelegate(IntPtr unitManagerPtr, IntPtr entityPtr);

    /// <summary>
    /// Create the scene manager binding.
    /// </summary>
    /// <param name="bindingManager">The binding manager.</param>
    /// <param name="bindingOptions">The options for the binding.</param>
    /// <returns>A network binding or an error.</returns>
    public static Result<UnitManagerBinding> Create
        (NosBindingManager bindingManager, UnitManagerBindingOptions bindingOptions)
    {
        var process = Process.GetCurrentProcess();

        var unitManagerStaticAddress = bindingManager.Scanner.CompiledFindPattern(bindingOptions.UnitManagerPattern);
        if (!unitManagerStaticAddress.Found)
        {
            return new BindingNotFoundError(bindingOptions.UnitManagerPattern, "UnitManagerBinding.UnitManager");
        }

        var focusEntityAddress = bindingManager.Scanner.CompiledFindPattern(bindingOptions.FocusEntityPattern);
        if (!focusEntityAddress.Found)
        {
            return new BindingNotFoundError(bindingOptions.FocusEntityPattern, "UnitManagerBinding.FocusEntity");
        }

        var focusEntityFunction = bindingManager.Hooks.CreateFunction<FocusEntityDelegate>
            (focusEntityAddress.Offset + (int)process.MainModule!.BaseAddress + 0x04);
        var focusEntityWrapper = focusEntityFunction.GetWrapper();

        var binding = new UnitManagerBinding
        (
            bindingManager,
            (int)process.MainModule!.BaseAddress + unitManagerStaticAddress.Offset,
            bindingOptions.UnitManagerOffsets,
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

    private readonly int _staticUnitManagerAddress;
    private readonly int[] _unitManagerOffsets;

    private readonly NosBindingManager _bindingManager;
    private FocusEntityDelegate _originalFocusEntity;

    private IHook<FocusEntityDelegate>? _focusHook;

    private UnitManagerBinding
    (
        NosBindingManager bindingManager,
        int staticUnitManagerAddress,
        int[] unitManagerOffsets,
        FocusEntityDelegate originalFocusEntity
    )
    {
        _originalFocusEntity = originalFocusEntity;
        _bindingManager = bindingManager;
        _staticUnitManagerAddress = staticUnitManagerAddress;
        _unitManagerOffsets = unitManagerOffsets;
    }

    /// <summary>
    /// Gets the address of unit manager.
    /// </summary>
    public IntPtr Address => _bindingManager.Memory.FollowStaticAddressOffsets
        (_staticUnitManagerAddress, _unitManagerOffsets);

    /// <summary>
    /// Event that is called when entity focus was called by NosTale.
    /// </summary>
    /// <remarks>
    /// The focus entity must be hooked for this event to be called.
    /// </remarks>
    public event Func<MapBaseObj?, bool>? EntityFocus;

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
            var res = _originalFocusEntity(Address, entityAddress);
            Console.WriteLine(res);
        }
        catch (Exception e)
        {
            return e;
        }

        return Result.FromSuccess();
    }

    private int FocusEntityDetour(IntPtr unitManagerPtr, IntPtr entityId)
    {
        MapBaseObj? obj = null;
        if (entityId != IntPtr.Zero)
        {
            obj = new MapBaseObj(_bindingManager.Memory, entityId);
        }

        var result = EntityFocus?.Invoke(obj);
        if (result ?? true)
        {
            var res = _originalFocusEntity(unitManagerPtr, entityId);
            Console.WriteLine(res);
            return res;
        }

        return 0;
    }
}