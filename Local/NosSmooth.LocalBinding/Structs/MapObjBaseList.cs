//
//  MapObjBaseList.cs
//
//  Copyright (c) František Boháček. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using Reloaded.Memory.Pointers;
using Reloaded.Memory.Sources;

namespace NosSmooth.LocalBinding.Structs;

/// <summary>
/// List of map objects.
/// </summary>
public class MapObjBaseList : IEnumerable<MapBaseObj>
{
    private readonly IMemory _memory;
    private readonly IntPtr _objListPointer;
    private readonly ArrayPtr<int> _objList;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapObjBaseList"/> class.
    /// </summary>
    /// <param name="memory">The memory.</param>
    /// <param name="objListPointer">The object list pointer.</param>
    public MapObjBaseList(IMemory memory, IntPtr objListPointer)
    {
        memory.Read(objListPointer + 0x04, out uint arrayFirst);
        _objList = new ArrayPtr<int>(arrayFirst, source: memory);
        _memory = memory;
        _objListPointer = objListPointer;
    }

    /// <summary>
    /// Gets the element at the given index.
    /// </summary>
    /// <param name="index">The index of the element.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown if the index is not in the bounds of the array.</exception>
    public MapBaseObj this[int index]
    {
        get
        {
            if (index >= Length || index < 0)
            {
                throw new IndexOutOfRangeException();
            }

            return new MapBaseObj(_memory, (IntPtr)_objList[index]);
        }
    }

    /// <summary>
    /// Gets the length of the array.
    /// </summary>
    public int Length
    {
        get
        {
            _memory.SafeRead(_objListPointer + 0x08, out int length);
            return length;
        }
    }

    /// <inheritdoc/>
    public IEnumerator<MapBaseObj> GetEnumerator()
    {
        return new MapObjBaseEnumerator(this);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private class MapObjBaseEnumerator : IEnumerator<MapBaseObj>
    {
        private readonly MapObjBaseList _list;
        private int _index;

        public MapObjBaseEnumerator(MapObjBaseList list)
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

        public MapBaseObj Current => _list[_index];

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }
    }
}