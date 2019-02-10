using System;

namespace Septerra.Core.AM
{
    public struct ABGRColor
    {
        public const Int32 SizeOf = 1 + BGRColor.SizeOf;

        public Byte Alpha;
        public BGRColor BGR;
    }
}