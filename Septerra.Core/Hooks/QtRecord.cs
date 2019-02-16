using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Septerra.Core.DB;

namespace Septerra.Core.Hooks
{
    public static unsafe class QtRecord
    {
        public static DbFsIndices Indices => Services<DbFsIndices>.Instance;

        private static readonly Dictionary<DbRecordId, SafeGCHandle> _pinnedNames = new Dictionary<DbRecordId, SafeGCHandle>();

        public static Boolean Find(UInt32 qtRecordId, out Byte* filePath, out UInt32 offset)
        {
            offset = 0;

            DbRecordId dbRecordId = new DbRecordId(qtRecordId);
            if (_pinnedNames.TryGetValue(dbRecordId, out SafeGCHandle ptr))
            {
                filePath = (Byte*)ptr.AddrOfPinnedObject();
                return true;
            }

            if (Indices.TryGetResourcePath(dbRecordId, out String resourcePath))
            {
                Int32 arraySize = Encoding.ASCII.GetByteCount(resourcePath) + 1; // \0
                Byte[] array = new Byte[arraySize];
                Encoding.ASCII.GetBytes(resourcePath, 0, resourcePath.Length, array, 0);

                SafeGCHandle handle = new SafeGCHandle(array, GCHandleType.Pinned);
                _pinnedNames[dbRecordId] = handle;

                filePath = (Byte*)handle.AddrOfPinnedObject();
                return true;
            }

            filePath = null;
            return false;
        }
    }
}