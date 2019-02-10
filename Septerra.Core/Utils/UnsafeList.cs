using System;
using System.IO;
using System.Linq;

namespace Septerra.Core
{
    public sealed class UnsafeList<T> where T : unmanaged
    {
        private T[] _buff;
        private Int32 _offset;

        public UnsafeList(Int32 capacity = 4)
        {
            _buff = new T[capacity];
        }

        public UnsafeList(ArraySegment<T> capacity)
        {
            if (capacity.Offset != 0)
                throw new NotSupportedException($"capacity.Offset != 0 ({capacity.Offset})");

            _buff = capacity.Array;
            _offset = capacity.Count;
        }

        public Int32 Count => _offset;

        //public void Add<T1>() where T1 : struct, T
        public void Add(T value)
        {
            EnsureCapacity(_offset + 1);

            _buff[_offset++] = value;
        }

        public void Clear()
        {
            _offset = 0;
        }

        public void EnsureCapacity(Int32 capacity)
        {
            var size = _buff.Length;
            if (size < capacity)
            {
                while (size < capacity)
                    size *= 2;

                Array.Resize(ref _buff, size);
            }
        }

        public ref T this[Int32 index] => ref _buff[index];

        public ArraySegment<T> GetBuffer()
        {
            return new ArraySegment<T>(_buff, 0, _offset);
        }

        public ArraySegment<T> GetBuffer(Int32 capacity)
        {
            EnsureCapacity(capacity);
            return new ArraySegment<T>(_buff, 0, _offset);
        }

        public T[] CopyToArray()
        {
            T[] result = new T[_offset];
            Buffer.BlockCopy(_buff, 0, result, 0, _offset);
            return result;
        }
    }
}