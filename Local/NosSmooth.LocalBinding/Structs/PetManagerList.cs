//
//  PetManagerList.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NosSmooth.LocalBinding.Errors;
using NosSmooth.LocalBinding.Extensions;
using NosSmooth.LocalBinding.Options;
using Reloaded.Memory.Sources;
using Remora.Results;
using SharpDisasm.Udis86;

namespace NosSmooth.LocalBinding.Structs;

/// <summary>
/// NosTale list of <see cref="PetManager"/>.
/// </summary>
public class PetManagerList : NostaleList<PetManager>
{
    /// <summary>
    /// Create <see cref="PlayerManager"/> instance.
    /// </summary>
    /// <param name="nosBrowserManager">The NosTale process browser.</param>
    /// <param name="options">The options.</param>
    /// <returns>The player manager or an error.</returns>
    public static Result<PetManagerList> Create(NosBrowserManager nosBrowserManager, PetManagerOptions options)
    {
        var characterObjectAddress = nosBrowserManager.Scanner.CompiledFindPattern(options.PetManagerListPattern);
        if (!characterObjectAddress.Found)
        {
            return new BindingNotFoundError(options.PetManagerListPattern, "PetManagerList");
        }

        if (nosBrowserManager.Process.MainModule is null)
        {
            return new NotFoundError("Cannot find the main module of the target process.");
        }

        int staticManagerAddress = (int)nosBrowserManager.Process.MainModule.BaseAddress + characterObjectAddress.Offset;
        return new PetManagerList(nosBrowserManager.Memory, staticManagerAddress, options.PetManagerListOffsets);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PetManagerList"/> class.
    /// </summary>
    /// <param name="memory">The memory.</param>
    /// <param name="staticPetManagerListAddress">The static pet manager address.</param>
    /// <param name="staticPetManagerOffsets">The offsets to follow to the pet manager list address.</param>
    public PetManagerList(IMemory memory, int staticPetManagerListAddress, int[] staticPetManagerOffsets)
        : base(memory, memory.FollowStaticAddressOffsets(staticPetManagerListAddress, staticPetManagerOffsets))
    {
    }
}