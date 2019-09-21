using System;
using System.Runtime.InteropServices;

namespace Septerra.Core.Hooks
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct ResourceName
    {
        public fixed sbyte name[20];
        public byte field_14;
        public byte field_15;
        public byte field_16;
        public byte field_17;
        public Int32 Index;
        public Int32 Count;
    };
}