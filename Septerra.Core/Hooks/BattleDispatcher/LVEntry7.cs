using System;
using System.Runtime.InteropServices;

namespace Septerra.Core.Hooks
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct LVEntry7
    {
        public ActorInfo2* ActorInfo;
        public Byte Type;
        public Byte CHIndexS1;
        public Int16 field_6;
        public Int32 f3;
        public Int32 f4;
        public Int16 ActorOX;
        public Int16 ActorOY;
        public Int32 f6;
        public Int16 f7;
        public Int16 f8;
        public Int32 ResourceId;
        public Int32 f9;
        public Int32 ScriptIndex;
    };
}