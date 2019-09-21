using System;
using System.Runtime.InteropServices;

namespace Septerra.Core.Hooks
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public unsafe struct ActorInfoBattleParams
    {
        public ActorInfoParams Params;
        public Int16 field_1A;
        public Int16 field_B0;
        public Int16 field_B2;
        public Int16 field_B4;
        public ActorATB PoisonTimer;
        public ActorATB DisableStatus2;
        public ActorATB Param03;
        public ActorATB Param04;
        public ActorATB Param05;
        public ActorATB DisableStatus1;
        public ActorATB Param07;
        public ActorATB Param08;
        public ActorATB Param09;
        public ActorATB Param10;
        public ActorATB Param11;
        public ActorATB Param12;
        public ActorATB Param13;
        public ActorATB Param14;
        public ActorATB Timer15;
        public Int32 CurrentHP;
        public Int32 SomeFlags;
        public Int16 ATB;
        public Byte field_A4;
        public Byte field_A5;
        public Byte field_A6;
        public Byte field_A7;
        public Byte field_A8;
        public Byte field_A9;
        public Int32 ExperienceToNextLevel;
        public Int16 field_AE;
        public Int32 field_140;
        public Int32 gap_144;
        public Int32 field_B8;
        public Int32 field_BC;
        public Int32 field_C0;
        public Int32 field_C4;
        public Int16 field_C8;
        public Int16 field_CA;
        public Int16 field_CC;
    };
}