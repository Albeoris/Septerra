using System;
using System.Collections.Generic;
using Septerra.Core;

namespace Septerra.Core.AM
{
    public unsafe struct AMAnimation
    {
        public const Int32 MaxShifts = 5;
        public const Int32 MaxFrames = 8;

        /* 00000000 */ public Int32 ShiftNumber;
        /* 00000004 */ private fixed Byte _shifts[MaxShifts * AMAnimationShift.SizeOf];
        /* 0000002C */ private fixed Byte _frames[MaxFrames * AMFrameReference.SizeOf];

        public ref AMFrameReference FirstFrame => ref GetFrames(0);
        public ref AMFrameReference LastFrame => ref GetFrames(MaxFrames - 1);

        public ref AMFrameReference GetFrames(Int32 index)
        {
            Asserts.InRange(index, 0, MaxFrames - 1);

            fixed (Byte* ptr = _frames)
            {
                AMFrameReference* structPtr = (AMFrameReference*)ptr;
                return ref structPtr[index];
            }
        }

        public ref AMAnimationShift FirstShift => ref GetShift(0);
        public ref AMAnimationShift LastShift => ref GetShift(ShiftNumber - 1);

        public ref AMAnimationShift GetShift(Int32 index)
        {
            Asserts.InRange(index, 0, ShiftNumber - 1);

            fixed (Byte* ptr = _shifts)
            {
                AMAnimationShift* structPtr = (AMAnimationShift*)ptr;
                return ref structPtr[index];
            }
        }
    }
}