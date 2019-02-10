using System;

namespace Septerra.Core.AM
{
    public struct BGRColor
    {
        public static readonly BGRColor Magenta = new BGRColor(255, 0, 255);

        public const Int32 SizeOf = 3;

        public Byte Blue;
        public Byte Green;
        public Byte Red;

        public BGRColor(Byte r, Byte g, Byte b)
        {
            Red = r;
            Green = g;
            Blue = b;
        }

        public override Boolean Equals(Object obj)
        {
            if (obj is BGRColor color)
                return Equals(color);
            return false;
        }

        public Boolean Equals(BGRColor other)
        {
            return Blue == other.Blue && Green == other.Green && Red == other.Red;
        }

        public override Int32 GetHashCode()
        {
            unchecked
            {
                var hashCode = Blue.GetHashCode();
                hashCode = (hashCode * 397) ^ Green.GetHashCode();
                hashCode = (hashCode * 397) ^ Red.GetHashCode();
                return hashCode;
            }
        }

        public static Boolean operator ==(BGRColor left, BGRColor right)
        {
            return left.Equals(right);
        }

        public static Boolean operator !=(BGRColor left, BGRColor right)
        {
            return !left.Equals(right);
        }
    }
}