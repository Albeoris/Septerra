using System;

namespace Septera
{
    public unsafe struct CHSegment1
    {
        public fixed Int32 Unknown1[4]; // => 0x10
        public UInt32 ResourceId; // => 0x14
        public fixed Int32 Unknown3[3]; // => 0x20
        public fixed Int16 Unknown4[128]; // => 0x120

        public static String FormatResourceId(UInt32 resourceId)
        {
            if (resourceId == 0)
                return "ZERO";

            UInt32 _fff00000 = resourceId & 0xFFF00000; // eax
            UInt32 _000fffff = resourceId & 0xFFFFF;

            switch (_fff00000)
            {
                case 0x67800000:
                    return $"Sound{_000fffff:D3}";
                case 0x2000000u:
                    return $"Movie{_000fffff:D2}";
                case 0x3000000u:
                    return $"GVar{_000fffff:D3}";
                case 0x5000000u:
                    return $"Text{_000fffff:D3}";
                case 0x6000000u:
                    return $"CM{_000fffff:D3}";
                case 0x7000000u:
                    return $"IL{_000fffff:D3}";
            }

            if (_fff00000 == 0x8000000u)
            {
                UInt32 div1000 = _000fffff / 1000;

                if (div1000 > 25)
                {
                    var ch1 = (Char)(_000fffff / 100000 + 65);
                    var ch2 = (Char)(_000fffff % 100000 / 1000 + 65);
                    var num = _000fffff % 100000 % 1000;
                    return $"{ch1}{ch2}{num:d3}";
                }
                else
                {
                    var ch1 = (Char)(div1000 + 65);
                    var num = _000fffff % 1000;
                    return $"{ch1}{num:d3}";
                }
            }

            if (resourceId < 0x1000000)
            {
                var ch1 = (Char)(resourceId / 1000 + 65);
                var num = resourceId % 1000;
                return $"{ch1}{num:d3}";
            }

            if ((resourceId & 0xFCFFFFFF) >= 0x1000000)
            {
                var ch1 = (Char)(resourceId % 100000000 / 1000 + 65);
                var num = resourceId % 100000000 % 1000;
                var ch2 = (Char)(resourceId / 100000000 + 64);
                return $"B{ch1}{num:d3}{ch2}";
            }
            else
            {
                var ch1 = (Char)((resourceId & 0xFCFFFFFF) / 1000 + 65);
                var num = (resourceId & 0xFCFFFFFF) % 1000;
                return $"{ch1}{num:d3}";
            }
        }

        public override String ToString()
        {
            return FormatResourceId(ResourceId);
        }
    }
}