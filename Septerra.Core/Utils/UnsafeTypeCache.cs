﻿using System;
using System.Runtime.InteropServices;

namespace Septerra.Core
{
    public static class UnsafeTypeCache<T> where T : unmanaged
    {
        public static readonly unsafe Int32 UnsafeSize = sizeof(T);
        public static readonly UIntPtr ArrayTypePointer = GetArrayTypePointer();

        private static unsafe UIntPtr GetArrayTypePointer()
        {
            T[] result = new T[1];
            using (SafeGCHandle handle = new SafeGCHandle(result, GCHandleType.Pinned))
                return *(((UIntPtr*)handle.AddrOfPinnedObject().ToPointer()) - 2);
        }

        public static IDisposable ChangeArrayType(Array array, Int32 oldElementSize)
        {
            unsafe
            {
                void* pinned;
                return ChangeArrayType(array, oldElementSize, out pinned);
            }
        }

        public static unsafe IDisposable ChangeArrayType(Array array, Int32 oldElementSize, out void* pointer)
        {
            if (array.Length < 1)
                throw new NotSupportedException();

            SafeGCHandle handle = new SafeGCHandle(array, GCHandleType.Pinned);
            try
            {
                pointer = handle.AddrOfPinnedObject().ToPointer();
                UIntPtr* arrayPointer = (UIntPtr*)pointer;
                UIntPtr arrayLength = *(arrayPointer - 1);
                UIntPtr arrayType = *(arrayPointer - 2);
                UInt64 arraySize = ((UInt64)arrayLength * (UInt64)oldElementSize);

                if (arraySize % (UInt64)UnsafeSize != 0)
                    throw new InvalidCastException();

                try
                {
                    *(arrayPointer - 1) = new UIntPtr(arraySize / (UInt64)UnsafeSize);
                    *(arrayPointer - 2) = ArrayTypePointer;

                    return new DisposableAction(() =>
                    {
                        *(arrayPointer - 1) = arrayLength;
                        *(arrayPointer - 2) = arrayType;
                        handle.Dispose();
                    });
                }
                catch
                {
                    *(arrayPointer - 1) = arrayLength;
                    *(arrayPointer - 2) = arrayType;
                    throw;
                }
            }
            catch
            {
                handle.Dispose();
                throw;
            }
        }
    }
}