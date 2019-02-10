using System;

namespace Septerra.Core.AM
{
    public unsafe struct AMFrame
    {
        public UInt16 ImageIndex;
        public UInt16 PalleteIndex;
        public UInt16 FrameTime;
        public Int16 MinusOX;
        public Int16 MinusOY;
        public UInt16 FlipImageFlags;
        public UInt16 Flags;
        public Int16 OffsetOX;
        public Int16 OffsetOY;
        public UInt16 AnimationMirrorIndexFrom0To4; // Araym's flying hands
        public UInt16 MaskImageIndex;
        public Int16 PlusOX;
        public Int16 PlusOY;

        public Boolean FlipHorizontal
        {
            get => (FlipImageFlags & 1) == 1;
            set
            {
                if (value)
                    FlipImageFlags |= 1;
                else
                    FlipImageFlags = (UInt16)(FlipImageFlags & ~1);
            }
        }

        public Boolean FlipVertical
        {
            get => (FlipImageFlags & 2) == 2;
            set
            {
                if (value)
                    FlipImageFlags |= 2;
                else
                    FlipImageFlags = (UInt16)(FlipImageFlags & ~2);
            }
        }
    }
}