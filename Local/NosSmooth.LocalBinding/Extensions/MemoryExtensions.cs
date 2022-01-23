//
//  MemoryExtensions.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Reloaded.Memory.Sources;

namespace NosSmooth.LocalBinding.Extensions;

/// <summary>
/// Extension methods for <see cref="IMemory"/>.
/// </summary>
public static class MemoryExtensions
{
    /// <summary>
    /// Follows the offsets to a 32-bit pointer.
    /// </summary>
    /// <param name="memory">The memory.</param>
    /// <param name="staticAddress">The static address to follow offsets from.</param>
    /// <param name="offsets">The offsets, first offset is the 0-th element.</param>
    /// <returns>A final address.</returns>
    public static IntPtr FollowStaticAddressOffsets(this IMemory memory, int staticAddress, int[] offsets)
    {
        int address = staticAddress;
        foreach (var offset in offsets)
        {
            memory.SafeRead((IntPtr)(address + offset), out address);
        }

        return (IntPtr)address;
    }
}