using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Septerra.Core.DB;
using Septerra.Core.Hooks;

namespace Septerra.Core.TX
{
    public sealed class TxFsService : IService
    {
        private readonly Object _lock = new Object();

        private readonly Dictionary<DbRecordId, TxCachedData> _byId = new Dictionary<DbRecordId, TxCachedData>();
        private readonly Dictionary<IntPtr, TxCachedData> _byPtr = new Dictionary<IntPtr, TxCachedData>();

        public DbFsIndices Indices => Services<DbFsIndices>.Instance;

        public IntPtr Acquire(DbRecordId txRecordId)
        {
            if (!Indices.TryGetResourcePath(txRecordId, out var resourcePath))
                throw new FileNotFoundException($"Cannot load unknown text resource: {txRecordId}");

            lock (_lock)
            {
                if (_byId.TryGetValue(txRecordId, out var cachedData))
                {
                    cachedData.Acquire();
                    return cachedData.Pointer;
                }
            }

            Byte[] fileContent = NativeFormatProvider.ReadAllBytes(resourcePath);

            IntPtr result;
            lock (_lock)
            {
                if (_byId.TryGetValue(txRecordId, out var cachedData))
                {
                    cachedData.Acquire();
                    return cachedData.Pointer;
                }

                cachedData = new TxCachedData(txRecordId, resourcePath, fileContent);
                result = cachedData.Pointer;

                _byId.Add(txRecordId, cachedData);
                _byPtr.Add(result, cachedData);
            }

            Log.Message($"Loaded resource [0x{txRecordId}, {resourcePath}].");
            return result;
        }

        public IntPtr Find(DbRecordId txRecordId)
        {
            lock (_lock)
            {
                if (_byId.TryGetValue(txRecordId, out var cachedData))
                    return cachedData.Pointer;
            }

            return IntPtr.Zero;
        }

        public void Release(IntPtr dataPtr)
        {
            lock (_lock)
            {
                if (_byPtr.TryGetValue(dataPtr, out var cachedData))
                    Release(cachedData);
            }
        }

        public void Release(DbRecordId txRecordId)
        {
            lock (_lock)
            {
                if (_byId.TryGetValue(txRecordId, out var cachedData))
                    Release(cachedData);
            }
        }

        private void Release(TxCachedData cachedData)
        {
            if (cachedData.Release() == 0)
            {
                _byId.Remove(cachedData.RecordId);
                _byPtr.Remove(cachedData.Pointer);

                cachedData.Dispose();
            }
        }
    }
}