//
//  NostaleList.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using Reloaded.Memory.Pointers;
using Reloaded.Memory.Sources;

namespace NosSmooth.LocalBinding.Structs;

/// <summary>
/// A class representing a list from nostale.
/// </summary>
/// <typeparam name="T">The type.</typeparam>
public class NostaleList<T> : IEnumerable<T>
    where T : NostaleObject, new()
{
    private readonly IMemory _memory;

    /// <summary>
    /// Initializes a new instance of the <see cref="NostaleList{T}"/> class.
    /// </summary>
    /// <param name="memory">The memory.</param>
    /// <param name="objListPointer">The object list pointer.</param>
    public NostaleList(IMemory memory, IntPtr objListPointer)
    {
        _memory = memory;
        Address = objListPointer;
    }

    /// <summary>
    /// Gets the address.
    /// </summary>
    protected IntPtr Address { get; }

    /// <summary>
    /// Gets the element at the given index.
    /// </summary>
    /// <param name="index">The index of the element.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown if the index is not in the bounds of the array.</exception>
    public T this[int index]
    {
        get
        {
            if (index >= Length || index < 0)
            {
                throw new IndexOutOfRangeException();
            }

            _memory.SafeRead(Address + 0x04, out int arrayAddress);
            _memory.SafeRead((IntPtr)arrayAddress + (0x04 * index), out int objectAddress);

            return new T
            {
                Memory = _memory,
                Address = (IntPtr)objectAddress
            };
        }
    }

    /// <summary>
    /// Gets the length of the array.
    /// </summary>
    public int Length
    {
        get
        {
            _memory.SafeRead(Address + 0x08, out int length);
            return length;
        }
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return new NostaleListEnumerator(this);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private class NostaleListEnumerator : IEnumerator<T>
    {
        private readonly NostaleList<T> _list;
        private int _index;

        public NostaleListEnumerator(NostaleList<T> list)
        {
            _index = -1;
            _list = list;
        }

        public bool MoveNext()
        {
            if (_list.Length > _index + 1)
            {
                _index++;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            _index = -1;
        }

        public T Current => _list[_index];

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }
    }
}