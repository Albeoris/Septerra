using System;
using Septerra.Core.DB;
using Septerra.Core.TX;
// ReSharper disable UnusedMember.Global

namespace Septerra.Core.Hooks
{
    public static unsafe class TxRecord
    {
        public static TxFsService TxSerivce => Services<TxFsService>.Instance;

        public static Boolean TryAcquire(UInt32 txRecordId, Int32 realSize, out Byte* cachedData)
        {
            cachedData = (Byte*)TxSerivce.Acquire(new DbRecordId(txRecordId));
            return true;
        }

        public static Boolean TryFind(UInt32 txRecordId, out Byte* cachedData)
        {
            cachedData = (Byte*)TxSerivce.Find(new DbRecordId(txRecordId));
            return true;
        }

        public static Boolean TryReleaseByPointer(Byte* cachedData)
        {
            TxSerivce.Release((IntPtr)cachedData);
            return true;
        }

        public static Boolean TryReleaseByResourceId(UInt32 txRecordId)
        {
            TxSerivce.Release(new DbRecordId(txRecordId));
            return true;
        }
    }
}