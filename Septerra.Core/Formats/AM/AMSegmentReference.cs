using System;

namespace Septerra.Core.AM
{
    public struct AMFrameReference
    {
        public const Int32 SizeOf = 4;

        /* 00000000 */ public UInt16 FrameIndex;
        /* 00000002 */ public UInt16 FrameCount;
    }
}