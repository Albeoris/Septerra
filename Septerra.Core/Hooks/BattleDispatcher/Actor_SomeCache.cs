using System;
using System.Runtime.InteropServices;

namespace Septerra.Core.Hooks
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct Actor_SomeCache
    {
        public Actor_SomeCache* F1;
        public Actor_SomeCache* F2;
        public ActorInfo2* Actor;
        public Actor_SomeAnimationDescriptor* field_C;
        public fixed Byte gap_10[80];
        public byte F4;
        public byte field_61;
        public byte field_62;
        public byte field_63;
    };
}