using System;
using System.Collections.Generic;

namespace Septerra.Core;

public readonly struct UnsafeArray<T> where T : unmanaged
{
    private readonly unsafe T* _ptr;
    private readonly Int32 _size;

    public unsafe UnsafeArray(T* ptr, Int32 size)
    {
        _ptr = ptr;
        _size = size;
    }

    public Int32 Count => _size;

    public unsafe T* this[Int32 index]
    {
        get
        {
            CheckIndexOutOfRange(index);
            return _ptr + index;
        }
    }

    private void CheckIndexOutOfRange(Int32 index)
    {
        if (index < 0 || index >= _size)
            throw new ArgumentOutOfRangeException(nameof(index), index, $"Index {index} is out of range [0...{_size}]");
    }
}