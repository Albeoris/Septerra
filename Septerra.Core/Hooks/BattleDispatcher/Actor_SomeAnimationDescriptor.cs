using System;
using System.Runtime.InteropServices;

namespace Septerra.Core.Hooks
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct Actor_SomeAnimationDescriptor
    {
        public Actor_SomeCache* Cache;
        public Int16 field_4;
        public UInt16 field_6;
        public Int16 field_8;
        public Int16 field_A;
        public float field_C;
        public float field_10;
        public float field_14;
        public float field_18;
        public float field_1C;
        public float field_20;
        public float field_24;
        public float field_28;
        public float field_2C;
        public Byte field_30_SomeType;
        public Byte gap_31;
        public Int16 field_32;
        public LVEntry21* LV21;
        public Int32 gap_38_1;
        public Int32 gap_38_2;
        public Int32 gap_38_3;
        public Int32 gap_38_4;
        public Int64 ElapsedGameTime;
        public Actor_SomeAnimationDescriptor* field_50;
        public Int32 LV7Index;
    };
}