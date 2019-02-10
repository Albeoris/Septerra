using System;
using Septerra.Core;

namespace Septerra.Core.AM
{
    public unsafe struct AMPalette
    {
        public const Int32 ColorNumber = 256;

        /* 00000000 */ private fixed Byte _data[ColorNumber * ABGRColor.SizeOf];

        public ref ABGRColor FirstColor => ref GetColor(0);
        public ref ABGRColor LastColor => ref GetColor(ColorNumber - 1);

        public ref ABGRColor GetColor(Int32 index)
        {
            Asserts.InRange(index, 0, ColorNumber - 1);

            fixed (Byte* ptr = _data)
            {
                ABGRColor* structPtr = (ABGRColor*)ptr;
                return ref structPtr[index];
            }
        }
    }
}