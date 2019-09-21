using System;
using System.Runtime.InteropServices;

namespace Septerra.Core.Hooks
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct  LVEntry21
    {
        public Int32 field_d00;
        public Int32 field_d04;
        public Int32 field_d08;
        public Int32 field_d12;
        public Int32 field_d16;
        public int gap_d20;
        public int field_18;
        public int field_1C;
        public int field_20;
        public Int32 field_d36;
        public Int32 field_d40;
        public Int32 field_d44;
        public int gap_d48;
        public int field_34;
        public int field_38;
        public int field_3C;
        public Int32 field_d58;
        public Int32 field_d62;
        public fixed byte Data[134];
        public int field_D0;
    };
}