using System;
using System.Runtime.InteropServices;

namespace Septerra.Core.Hooks
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public unsafe struct ActorInfoParams
    {
        public UInt16 Level;
        public UInt16 Strength;
        public UInt16 Vitality;
        public UInt16 Agility;
        public UInt16 Psyche;
        public UInt16 Perception;
        public UInt16 field_A0;
        public UInt16 MaxHP;
        public UInt16 Strike;
        public Int16 Armor;
        public Int16 Power;
        public Int16 Core;
        public Int16 Speed;
    };
}