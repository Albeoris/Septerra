using System;

namespace Septera
{
    public unsafe struct CHSegment1
    {
        public fixed Int32 Unknown1[8]; // => 0x20
        public fixed Int16 Unknown2[128]; // => 0x120
    }
}