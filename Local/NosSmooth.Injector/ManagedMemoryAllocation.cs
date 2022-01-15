//
//  ManagedMemoryAllocation.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net.NetworkInformation;
using Reloaded.Memory.Sources;

namespace NosSmooth.Injector;

/// <summary>
/// Represents freeable memory allocation.
/// </summary>
internal class ManagedMemoryAllocation : IDisposable
{
    private readonly IMemory _memory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManagedMemoryAllocation"/> class.
    /// </summary>
    /// <param name="memory">The memory with allocation.</param>
    /// <param name="pointer">The pointer to allocated memory.</param>
    public ManagedMemoryAllocation(IMemory memory, IntPtr pointer)
    {
        Pointer = pointer;
        _memory = memory;

    }

    /// <summary>
    /// The allocated pointer number.
    /// </summary>
    public IntPtr Pointer { get; private set; }

    /// <summary>
    /// Whether the memory is currently allocated.
    /// </summary>
    public bool Allocated => Pointer != IntPtr.Zero;

    /// <inheritdoc />
    public void Dispose()
    {
        _memory.Free(Pointer);
        Pointer = IntPtr.Zero;
    }
}