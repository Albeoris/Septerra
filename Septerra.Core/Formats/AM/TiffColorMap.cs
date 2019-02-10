using System;

namespace Septerra.Core.AM
{
    public sealed class TiffColorMap
    {
        public readonly UInt16[] Red;
        public readonly UInt16[] Green;
        public readonly UInt16[] Blue;

        public TiffColorMap(Int32 colorNumber)
        {
            Red = new UInt16[colorNumber];
            Green = new UInt16[colorNumber];
            Blue = new UInt16[colorNumber];
        }

        public BGRColor this[Int32 index]
        {
            get => GetColor(index);
            set => SetColor(index, value);
        }

        private BGRColor GetColor(Int32 index)
        {
            return new BGRColor(

                ConvertTo8Bit(Red[index]),
                ConvertTo8Bit(Green[index]),
                ConvertTo8Bit(Blue[index])
            );
        }

        private void SetColor(Int32 index, BGRColor value)
        {
            Red[index] = ConvertTo16Bit(value.Red);
            Green[index] = ConvertTo16Bit(value.Green);
            Blue[index] = ConvertTo16Bit(value.Blue);
        }

        public static Byte ConvertTo8Bit(UInt16 value)
        {
            return (Byte)(value * Byte.MaxValue / UInt16.MaxValue);
        }

        public static UInt16 ConvertTo16Bit(Byte value)
        {
            return (UInt16)(value * UInt16.MaxValue / Byte.MaxValue);
        }
    }
}