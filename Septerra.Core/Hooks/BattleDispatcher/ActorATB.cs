using System;
using System.Runtime.InteropServices;

namespace Septerra.Core.Hooks
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public unsafe struct ActorATB
    {
        public Int16 ATBValue;
        public Int16 ATBSegmentCount;
        public Int16 ATBPreviousSegmentCount;
        public UInt16 Value;
    };  
}