using System;
using System.Collections.Generic;

namespace Septera
{
    public static class TXEncoding
    {
        public static String ToText(Byte bt)
        {
            if (ToChar.TryGetValue(bt, out char value))
                return value.ToString();

            return $"[{bt:X2}]";
            //throw new NotSupportedException($"[{bt:X2}]");
        }

        private static Dictionary<Byte, Char> ToChar = new Dictionary<byte, char>()
        {
            {0x01, 'a'},
            {0x02, 'b'},
            {0x03, 'c'},
            {0x04, 'd'},
            {0x05, 'e'},
            {0x06, 'f'},
            {0x07, 'g'},
            {0x08, 'h'},
            {0x09, 'i'},
            {0x0A, 'j'},
            {0x0B, 'k'},
            {0x0C, 'l'},
            {0x0D, 'm'},
            {0x0E, 'n'},
            {0x0F, 'o'},
            {0x10, 'p'},
            {0x11, 'q'},
            {0x12, 'r'},
            {0x13, 's'},
            {0x14, 't'},
            {0x15, 'u'},
            {0x16, 'v'},
            {0x17, 'w'},
            {0x18, 'x'},
            {0x19, 'y'},
            {0x1A, 'z'},

            {0x1F, 'A'},
            {0x20, 'B'},
            {0x21, 'C'},
            {0x22, 'D'},
            {0x23, 'E'},
            {0x24, 'F'},
            {0x25, 'G'},
            {0x26, 'H'},
            {0x27, 'I'},
            {0x28, 'J'},
            {0x29, 'K'},
            {0x2A, 'L'},
            {0x2B, 'M'},
            {0x2C, 'N'},
            {0x2D, 'O'},
            {0x2E, 'P'},
            {0x2F, 'Q'},
            {0x30, 'R'},
            {0x31, 'S'},
            {0x32, 'T'},
            {0x33, 'U'},
            {0x34, 'V'},
            {0x35, 'W'},
            {0x36, 'X'},
            {0x37, 'Y'},
            {0x38, 'Z'},

            {0x3C, ','},
            {0x3D, ':'},
            {0x3E, '\''},
            {0x3F, '!'},

            {0x40, '-'},
            {0x41, '0'},
            {0x42, '1'},
            {0x43, '2'},
            {0x44, '3'},
            {0x45, '4'},
            {0x46, '5'},
            {0x47, '6'},
            {0x48, '7'},
            {0x49, '8'},
            {0x4A, '9'},
            {0x4B, '('},
            {0x4C, ')'},
            {0x4D, '.'},
            {0x4E, '?'},
            {0x4F, '"'},

            {0x50, ';'},

            {0xFE, '\n'},

            {0xFF, ' '}
        };
    }
}