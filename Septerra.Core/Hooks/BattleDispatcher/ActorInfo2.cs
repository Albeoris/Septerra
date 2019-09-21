using System;
using System.Runtime.InteropServices;

namespace Septerra.Core.Hooks
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public unsafe struct ActorInfo2
    {
        public byte* InternalName;
        public int field_4;
        public Int64 ScaledAnimationDelay;
        public Int32 AnimationId;
        public Int32 field_14;
        public Int32 SomeFlags;
        public Int32 actorIndex;
        public PointF Pos1;
        public PointF Pos2;
        public PointF Pos3;
        public PointF Pos4;
        public IntPtr Animation;
        public Int16 AnimationS2_Offset;
        public Int16 gap_46;
        public Int32 AnimationS1_LinksS2_Offset;
        public Int32 AnimationS1_LinksS2_Offset2;
        public Int32 field_50;
        public float field_54;
        public Byte field_58;
        public Byte field_59;
        public Byte field_5A;
        public Byte field_5B;
        public Int32 AnimationS1_LinksS2_Index;
        public Int32 AnimationS1_LinksS2_Index2;
        public float AltPositionOY;
        public Int32 AnimationSegment2MinusOffset;
        public Int32 field_6C;
        public Int32 field_70;
        public Int32 field_74;
        public Int32 intersectedObject;
        public Int32 field_7C;
        public ActorInfo2* linkedActor;
        public Int32 field_84;
        public ActorInfo2* linkedActor2;
        public Int32 InitialPositionOX;
        public Int32 InitialPositionOY;
        public ActorInfoBattleParams Battle;
        public Int16 field_162;
        public CHHeader* chHeader;
        public UInt16 CHIndexS1;
        public Int16 field_168;
        public SceneInstance* SceneInstance;
        public Int16 field_170;
        public Int16 field_172;
        public float Segment1_FieldF8;
        public float Segment1_FieldF8_Percent60;
        public float field_178;
        public ActorInfo2* LinkdedActor;
        public SceneInstance* field_184;
        public Int64 gap_188;
        public UInt16 PaletteIndex;
        public UInt16 gap_192;
        public IntPtr DecodedPalette;
        public int field_198;
        public byte field_19C;
        public byte field_19D;
        public byte field_19E;
        public byte field_19F;
        public Actor_SomeAnimationDescriptor* SomeAnimationDescriptor;
        public LVEntry7* LV7;
        public int gap_1A8;
        public int field_1AC;
        public Int32 field_1B0_1;
        public Int32 field_1B0_2;
        public Int32 field_1B0_3;
        public Int32 field_1B0_4;
        public double AnimationTimeScale;
    };
}