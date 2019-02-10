using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Septerra.Core;

namespace Septerra.Core.AM
{
    public struct AMHeader
    {
        public const UInt16 KnownMagicNumber = 0x4D41;
        public const UInt16 KnownVersion = 0x3430;

        /* 00000000 */ public UInt16 MagicNumber;
        /* 00000002 */ public UInt16 VersionNumber;
        /* 00000004 */ public Int32 AnimationType;
        /* 00000008 */ public Int32 AnimationOffset;
        /* 0000000C */ public Int32 AnimationCount;
        /* 00000010 */ public Int32 FrameOffset;
        /* 00000014 */ public Int32 FrameCount;
        /* 00000018 */ public Int32 PaletteOffset;
        /* 0000001C */ public Int32 PaletteCount;
        /* 00000020 */ public Int32 ImageHeaderOffset;
        /* 00000024 */ public Int32 ImageHeaderCount;
        /* 00000028 */ public Int32 ImageContentOffset;
        /* 0000002C */ public Int32 ImageContentSize;
        /* 00000030 */ public Int32 ImageSegmentOffset;
        /* 00000034 */ public Int32 ImageSegmentCount;
        /* 00000038 */ public Int32 ImageLineOffset;
        /* 0000003C */ public Int32 ImageLineCount;
        /* 00000040 */ public Int32 Zero1;
        /* 00000044 */ public Int32 Zero2;
    }
}