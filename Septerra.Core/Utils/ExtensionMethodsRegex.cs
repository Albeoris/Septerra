using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Septerra.Core
{
    public static class ExtensionMethodsRegex
    {
        public static Decimal ToDecimal(this Match match, Int32 index)
        {
            String value = ToString(match, index);
            if (Decimal.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var result))
                return result;

            throw new FormatException($"Cannot parse [{value}] as a decimal value.");
        }

        public static Int32 ToInt32(this Match match, Int32 index)
        {
            String value = ToString(match, index);
            if (Int32.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
                return result;

            throw new FormatException($"Cannot parse [{value}] as a Int32 value.");
        }

        public static String ToString(Match match, Int32 index)
        {
            if (match.Groups.Count <= index)
                throw new ArgumentOutOfRangeException(nameof(index), $"Cannot find {index} match group to extract string. There is {match.Groups.Count} entries.");

            return match.Groups[index].Value;
        }
    }
}