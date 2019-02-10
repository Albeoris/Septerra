using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Septerra;
using Septerra.Core;
using Septerra.Core.DB;
using Septerra.Core.Hooks;

namespace Septerra.Core.TX
{
    public sealed class TxCachedData : IDisposable
    {
        private readonly IDisposable _unregistrator;
        private SafeGCHandle _gcHandle;
        private Int32 _usageCount = 1;

        public DbRecordId RecordId { get; }
        public IntPtr Pointer { get; }

        private readonly Byte[] _buff;

        public TxCachedData(in DbRecordId txRecordId, String resourcePath, Byte[] fileContent)
        {
            RecordId = txRecordId;

            _buff = new Byte[fileContent.Length * 2]; // For updates
            Array.Copy(fileContent, _buff, fileContent.Length);

            _gcHandle = new SafeGCHandle(_buff, GCHandleType.Pinned);
            _unregistrator = Services<FileSystemWatchService>.Instance.Register(resourcePath, Refresh);

            Pointer = _gcHandle.AddrOfPinnedObject();
            RefreshPointers();
        }

        public Int32 Acquire()
        {
            return ++_usageCount;
        }

        public Int32 Release()
        {
            return --_usageCount;
        }

        public void Dispose()
        {
            _unregistrator.Dispose();
            _gcHandle.Dispose();
        }

        private void Refresh(FileContentProvider provider)
        {
            if (provider.Length > _buff.Length)
            {
                Log.Warning($"Cannot handle changed content of the text resource [0x{RecordId}]. Not enough buffer size: {_buff.Length} of {provider.Length}.");
                return;
            }

            Byte[] array = provider.GetArray();

            unsafe
            {
                Kernel32.ZeroMemory(Pointer.ToPointer(), _buff.Length);
            }

            Array.Copy(array, _buff, array.Length);
            RefreshPointers();
        }

        private void RefreshPointers()
        {
            unsafe
            {
                void* ptr = Pointer.ToPointer();
                TXHeader header = *(TXHeader*)ptr;
                TXEntry* entryPtr = (TXEntry*)(Pointer + sizeof(TXHeader));
                for (int i = 0; i < header.Count; i++)
                    (entryPtr + i)->Offset += (UInt32)Pointer;
            }
        }
    }
}