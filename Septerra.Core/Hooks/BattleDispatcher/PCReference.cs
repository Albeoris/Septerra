using System;
using System.Runtime.InteropServices;

namespace Septerra.Core.Hooks
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct PCReference
    {
        public Int16 SlotIndex;
        public Int16 PCharacterIndex;
        public ActorInfo2* Actor;
    };
}