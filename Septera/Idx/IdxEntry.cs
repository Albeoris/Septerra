using System;

namespace Septera
{
    public sealed class IdxEntry
    {
        public Int32 ResourceIndex { get; }
        public UInt32 ResourceId { get; }
        public Int32 Offset { get; }
        public Int32 CompressedSize { get; }
        public Int32 UncompressedSize { get; }
        public Int64 ModifiedTime { get; }

        public Boolean IsCompressed => CompressedSize != UncompressedSize;

        public IdxEntry(Int32 resourceIndex, UInt32 resourceId, Int32 offset, Int32 compressedSize, Int32 uncompressedSize, Int64 modifiedTime)
        {
            ResourceIndex = resourceIndex;
            ResourceId = resourceId;
            Offset = offset;
            CompressedSize = compressedSize;
            UncompressedSize = uncompressedSize;
            ModifiedTime = modifiedTime;
        }
    }
}