using System;
using System.Runtime.InteropServices;

namespace Septerra.Core.Hooks
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct SceneInstance
    {
        public ResourceName* lvEventName;
        public Int32 eventId;
        public Int32 field_8;
        public Int32 field_C;
        public Int32 field_10;
        public ActorInfo2* Actor;
        public SC_CombatListEntry* BattleTarget;
        public IntPtr ScriptInstance;
        public byte Flags;
        public byte actor_field84;
        public Int16 field_22;
        public IntPtr lvHeader;
        public LVEntry7* lv7;
        public IntPtr lv18;
        public IntPtr LV19;
        public Int32* lv20;
        public LVEntry21* lv21;
        public Int32 lv7Count;
        public Int32 lv18Count;
        public Int32 lv19Count;
        public Int32 lv20Count;
        public Int32 field_4C;
        public Int32 lv18Entry_Count;
        public Int32 field_54;
        public Int32 PCIndex;
        public PCReference PCReference1;
        public PCReference PCReference2;
        public PCReference PCReference3;
        public Int32 field_74;
        public IntPtr SceneObject;
    };
}