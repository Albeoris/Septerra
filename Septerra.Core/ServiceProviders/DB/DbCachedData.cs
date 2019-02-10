using System;
using Septerra.Core;
using Septerra.Core.Hooks;

namespace Septerra.Core.DB
{
    public sealed class DbCachedData : IDisposable
    {
        public readonly DbRecordId DbRecordId;
        public readonly String FilePath;

        private Byte[] _data;
        private Int32 _offset;
        private readonly IDisposable _unregistrator;

        public DbCachedData(DbRecordId dbRecordId, String filePath, Byte[] data)
        {
            DbRecordId = dbRecordId;
            FilePath = filePath;
            _data = data;

            _unregistrator = Services<FileSystemWatchService>.Instance.Register(filePath, Refresh);
        }

        public Int32 Size => _data.Length;

        public void Seek(Int32 offset)
        {
            Asserts.InRange(offset, 0, Size);

            _offset = offset;
        }

        public unsafe Int32 Read(Byte* output, Int32 outputSize)
        {
            Int32 size = Math.Min(Size - _offset, Asserts.Positive(outputSize));
            if (size == 0)
                return size;

            fixed (Byte* dataPtr = _data)
            {
                Byte* input = dataPtr + _offset;

                for (Int32 i = 0; i < size; i++)
                    output[i] = input[i];
            }

            return size;
        }

        public void Refresh(FileContentProvider contentProvider)
        {
            _data = contentProvider.GetArray();
        }

        public void Dispose()
        {
            _unregistrator?.Dispose();
        }
    }
}