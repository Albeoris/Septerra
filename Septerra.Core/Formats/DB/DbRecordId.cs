using System;
using System.Globalization;

namespace Septerra.Core.DB
{
    public readonly struct DbRecordId
    {
        public readonly UInt32 Value;

        public DbRecordId(UInt32 id)
        {
            Value = id;
        }

        public override Int32 GetHashCode() => Value.GetHashCode();
        public override Boolean Equals(Object other) => other is DbRecordId o && Value.Equals(o.Value);
        public override String ToString() => Value.ToString("X8");

        public static Boolean TryParse(String s, out DbRecordId result)
        {
            if (UInt32.TryParse(s, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var parsedValue))
            {
                result = new DbRecordId(parsedValue);
                return true;
            }

            result = default;
            return false;
        }

        public String Format()
        {
            if (Value == 0)
                return "ZERO";

            UInt32 _fff00000 = Value & 0xFFF00000; // eax
            UInt32 _000fffff = Value & 0xFFFFF;

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

            if (Value < 0x1000000)
            {
                var ch1 = (Char)(Value / 1000 + 65);
                var num = Value % 1000;
                return $"{ch1}{num:d3}";
            }

            if ((Value & 0xFCFFFFFF) >= 0x1000000)
            {
                var ch1 = (Char)(Value % 100000000 / 1000 + 65);
                var num = Value % 100000000 % 1000;
                var ch2 = (Char)(Value / 100000000 + 64);
                return $"B{ch1}{num:d3}{ch2}";
            }
            else
            {
                var ch1 = (Char)((Value & 0xFCFFFFFF) / 1000 + 65);
                var num = (Value & 0xFCFFFFFF) % 1000;
                return $"{ch1}{num:d3}";
            }
        }
    }
}