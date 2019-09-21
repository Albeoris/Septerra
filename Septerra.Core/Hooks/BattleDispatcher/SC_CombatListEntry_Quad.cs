using System;
using System.Runtime.InteropServices;

namespace Septerra.Core.Hooks
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct SC_CombatListEntry_Quad
    {
        public Int16 field_00;
        public Int16 field_04;
        public Int16 field_08;
        public Int16 field_0A;
    };
}