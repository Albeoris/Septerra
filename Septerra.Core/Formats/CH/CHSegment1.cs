using System;

namespace Septerra.Core
{
    public unsafe struct CHSegment1
    {
        public fixed Int32 Unknown1[4]; // => 0x10
        public UInt32 AnimationResourceId; // => 0x14
        public fixed Int32 Unknown3[3]; // => 0x20
        public fixed Int16 Unknown4[128]; // => 0x120
    }
}