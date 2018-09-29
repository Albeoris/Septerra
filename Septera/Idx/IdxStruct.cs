using System;

namespace Septera
{
    public struct IdxStruct
    {
        public UInt32 ResourceId;
        public Int32 Package;
        public Int32 Offset;
        public Int32 UncompressedSize;
        public Int32 Flags;
        public Int32 CompressedSize;
        public Int64 ModifiedTime;
    }
}