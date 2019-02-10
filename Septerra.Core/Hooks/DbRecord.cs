using System;
using Septerra.Core;
using Septerra.Core.DB;

// ReSharper disable UnusedMember.Global

namespace Septerra.Core.Hooks
{
    public static unsafe class DbRecord
    {
        public static DbFsService DbSerivce => Services<DbFsService>.Instance;

        public static Boolean TryOpen(UInt32 dbRecordId, out Int32 dbRecordIndex)
        {
            dbRecordIndex = DbSerivce.Open(new DbRecordId(dbRecordId)).Value;
            return true;
        }

        public static Boolean TryGetDecompressedSize(Int32 dbRecordIndex, out Int32 decompressedSize)
        {
            decompressedSize = DbSerivce.GetDecompressedSize(new DbCachedDataId(dbRecordIndex));
            return true;
        }

        public static Boolean TrySeek(Int32 dbRecordIndex, Int32 offset, Int32 seekType)
        {
            Asserts.Expected(seekType, 0);

            DbSerivce.Seek(new DbCachedDataId(dbRecordIndex), offset);
            return true;
        }

        public static Boolean TryRead(Int32 dbRecordIndex, Byte* output, Int32 outputSize, out Int32 readedSize)
        {
            readedSize = DbSerivce.Read(new DbCachedDataId(dbRecordIndex), output, outputSize);
            return true;
        }

        public static Boolean TryClose(Int32 dbRecordIndex)
        {
            DbSerivce.Close(new DbCachedDataId(dbRecordIndex));
            return true;
        }
    }
}