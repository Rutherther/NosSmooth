//
//  NostaleStringA.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using Reloaded.Memory.Sources;

namespace NosSmooth.LocalBinding.Objects;

/// <summary>
/// Represents nostale string object.
/// </summary>
public class NostaleStringA : IDisposable
{
    private readonly IMemory _memory;
    private IntPtr _pointer;

    /// <summary>
    /// Create an instance of <see cref="NostaleStringA"/>.
    /// </summary>
    /// <param name="memory">The memory to allocate the string on.</param>
    /// <param name="data">The string contents.</param>
    /// <returns>A nostale string.</returns>
    public static NostaleStringA Create(IMemory memory, string data)
    {
        var bytes = Encoding.ASCII.GetBytes(data);
        var allocated = memory.Allocate(bytes.Length + 1 + 8);
        memory.SafeWrite(allocated, 1);
        memory.SafeWrite(allocated + 4, data.Length);
        memory.SafeWriteRaw(allocated + 8, bytes);
        memory.SafeWrite(allocated + 8 + data.Length, 0);

        return new NostaleStringA(memory, allocated);
    }

    private NostaleStringA(IMemory memory, IntPtr pointer)
    {
        _memory = memory;
        _pointer = pointer;

    }

    /// <summary>
    /// Finalizes an instance of the <see cref="NostaleStringA"/> class.
    /// </summary>
    ~NostaleStringA()
    {
        Free();
    }

    /// <summary>
    /// Gets whether the string is still allocated.
    /// </summary>
    public bool Allocated => _pointer != IntPtr.Zero;

    /// <summary>
    /// Get the pointer to the string.
    /// </summary>
    /// <returns>A pointer to the string to pass to NosTale.</returns>
    public IntPtr Get()
    {
        return _pointer + 0x08;
    }

    /// <summary>
    /// Free the memory allocated by the string.
    /// </summary>
    public void Free()
    {
        if (Allocated)
        {
            _memory.Free(_pointer);
            _pointer = IntPtr.Zero;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Free();
    }
}