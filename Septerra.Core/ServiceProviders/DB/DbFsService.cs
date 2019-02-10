using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Septerra.Core.Hooks;

namespace Septerra.Core.DB
{
    public sealed class DbFsService : IService
    {
        private readonly DbFsIndices _indices;
        private readonly Dictionary<DbCachedDataId, DbCachedData> _byId = new Dictionary<DbCachedDataId, DbCachedData>();
        private Int32 _currentIndex = Int32.MinValue;

        public DbFsService()
        {
            _indices = Services<DbFsIndices>.Instance;
        }

        public DbCachedDataId Open(DbRecordId dbRecordId)
        {
            if (!_indices.TryGetResourcePath(dbRecordId, out var resourcePath))
                throw new FileNotFoundException($"Cannot load unknown resource: {dbRecordId}");

            DbCachedDataId dbRecordIndex;

            Byte[] fileContent = NativeFormatProvider.ReadAllBytes(resourcePath);
            lock (_byId)
            {
                dbRecordIndex = new DbCachedDataId(_currentIndex++);
                DbCachedData data = new DbCachedData(dbRecordId, resourcePath, fileContent);
                _byId[dbRecordIndex] = data;
            }

            Log.Message($"Loaded resource {dbRecordIndex}: [0x{dbRecordId}, {resourcePath}].");
            return dbRecordIndex;
        }

        public Int32 GetDecompressedSize(DbCachedDataId dbRecordIndex)
        {
            DbCachedData data = GetCachedData(dbRecordIndex);
            return data.Size;
        }

        public void Seek(DbCachedDataId dbRecordIndex, Int32 offset)
        {
            DbCachedData data = GetCachedData(dbRecordIndex);
            data.Seek(offset);
        }

        public unsafe Int32 Read(DbCachedDataId dbRecordIndex, Byte* output, Int32 outputSize)
        {
            DbCachedData data = GetCachedData(dbRecordIndex);
            return data.Read(output, outputSize);
        }

        public void Close(DbCachedDataId dbRecordIndex)
        {
            Boolean result;

            lock (_byId)
            {
                if (_byId.TryGetValue(dbRecordIndex, out DbCachedData data))
                {
                    _byId.Remove(dbRecordIndex);
                    data.Dispose();

                    result = true;
                }
                else
                {
                    result = false;
                }
            }

            if (!result)
                throw new FileNotFoundException($"Cannot close not loaded resource {dbRecordIndex}.");

            Log.Message($"Unloaded resource {dbRecordIndex}.");
        }

        private DbCachedData GetCachedData(DbCachedDataId dbRecordIndex)
        {
            lock (_byId)
            {
                if (_byId.TryGetValue(dbRecordIndex, out var data))
                    return data;
            }

            if (dbRecordIndex.Value < _currentIndex)
                throw new ObjectDisposedException($"The resource {dbRecordIndex} is already closed.");

            throw new FileNotFoundException($"The resource {dbRecordIndex} isn't opened yet.");
        }
    }
}