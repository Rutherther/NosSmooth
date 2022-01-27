//
//  PetManagerBinding.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.LocalBinding.Options;
using NosSmooth.LocalBinding.Structs;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;
using Remora.Results;

namespace NosSmooth.LocalBinding.Objects;

/// <summary>
/// The binding to NosTale pet manager.
/// </summary>
public class PetManagerBinding
{
    /// <summary>
    /// Create nostale pet manager binding.
    /// </summary>
    /// <param name="bindingManager">The binding manager.</param>
    /// <param name="petManagerList">The list of the pet managers.</param>
    /// <param name="options">The options.</param>
    /// <returns>A pet manager binding or and error.</returns>
    public static Result<PetManagerBinding> Create
        (NosBindingManager bindingManager, PetManagerList petManagerList, PetManagerBindingOptions options)
    {
        var petManager = new PetManagerBinding(petManagerList);
        var hookResult = bindingManager.CreateHookFromPattern<PetWalkDelegate>
        (
            "PetManagerBinding.PetWalk",
            petManager.PetWalkDetour,
            options.PetWalkPattern,
            hook: options.HookPetWalk
        );

        if (!hookResult.IsSuccess)
        {
            return Result<PetManagerBinding>.FromError(hookResult);
        }

        petManager._petWalkHook = hookResult.Entity;
        return petManager;
    }

    [Function
    (
        new[] { FunctionAttribute.Register.eax, FunctionAttribute.Register.edx, FunctionAttribute.Register.ecx },
        FunctionAttribute.Register.eax,
        FunctionAttribute.StackCleanup.Callee
    )]
    private delegate bool PetWalkDelegate
    (
        IntPtr petManagerPtr,
        uint position,
        short unknown0 = 0,
        int unknown1 = 1,
        int unknown2 = 1
    );

    private IHook<PetWalkDelegate> _petWalkHook = null!;

    private PetManagerBinding(PetManagerList petManagerList)
    {
        PetManagerList = petManagerList;
    }

    /// <summary>
    /// Gets the hook of the pet walk function.
    /// </summary>
    public IHook PetWalkHook => _petWalkHook;

    /// <summary>
    /// Gets pet manager list.
    /// </summary>
    public PetManagerList PetManagerList { get; }

    /// <summary>
    /// Walk the given pet to the given location.
    /// </summary>
    /// <param name="selector">Index of the pet to walk. -1 for every pet currently available.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <returns>A result returned from NosTale or an error.</returns>
    public Result<bool> PetWalk(int selector, ushort x, ushort y)
    {
        uint position = ((uint)y << 16) | x;
        if (PetManagerList.Length < selector + 1)
        {
            return new NotFoundError("Could not find the pet using the given selector.");
        }

        if (selector == -1)
        {
            bool lastResult = true;
            for (int i = 0; i < PetManagerList.Length; i++)
            {
                lastResult = _petWalkHook.OriginalFunction(PetManagerList[i].Address, position);
            }

            return lastResult;
        }
        else
        {
            return _petWalkHook.OriginalFunction(PetManagerList[selector].Address, position);
        }
    }

    private bool PetWalkDetour
    (
        IntPtr petManagerPtr,
        uint position,
        short unknown0 = 0,
        int unknown1 = 1,
        int unknown2 = 1
    )
    {
        return _petWalkHook.OriginalFunction
        (
            petManagerPtr,
            position,
            unknown0,
            unknown1,
            unknown2
        );
    }
}