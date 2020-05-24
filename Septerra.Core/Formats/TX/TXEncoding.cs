using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Septerra.Core
{
    public static class TXEncoding
    {
        public static Byte ToText(Byte bt)
        {
            if (ToChar.TryGetValue(bt, out Char value))
                return (Byte) value;

            if (_isRealCodepage)
                return 0xFF; // It's valid game logic

            throw new NotSupportedException($"[{bt:X2}]");
        }

        public static Byte ToText(Char bt)
        {
            if (ToByte.TryGetValue(bt, out Byte value))
                return value;

            throw new NotSupportedException($"[{checked((Byte) bt):X2}]");
        }

        private static readonly Dictionary<Byte, Char> ToChar = new Dictionary<Byte, Char>
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

            {0x1B, 'ä'},
            {0x1C, 'ß'},
            {0x1D, 'ö'},
            {0x1E, 'ü'},

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

            {0x39, 'Ä'},
            {0x3A, 'Ö'},
            {0x3B, 'Ü'},

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
            {0x51, '='},
            {0x52, '_'},
            {0x53, 'á'},
            {0x56, 'é'},
            {0x58, 'í'},
            {0x5A, 'ó'},
            {0x5E, 'ú'},

            {0x60, 'ý'},
            {0x62, 'Á'},
            {0x65, 'É'},
            {0x67, 'Í'},
            {0x69, 'Ó'},
            {0x6D, 'Ú'},
            {0x6F, 'Ý'},

            {0x71, 'à'},
            {0x72, 'â'},
            {0x73, 'ç'},
            {0x74, 'è'},
            {0x75, 'ë'},
            {0x76, 'ê'},
            {0x77, 'î'},
            {0x78, 'ï'},
            {0x79, 'ô'},
            {0x7B, 'û'},
            {0x7C, 'À'},
            {0x7D, 'Â'},
            {0x7E, 'Ç'},
            {0x7F, 'È'},

            {0x80, 'Ë'},
            {0x81, 'Ê'},
            {0x82, 'Î'},
            {0x83, 'Ï'},
            {0x84, 'Ô'},
            {0x86, 'Û'},

            {0x97, 'ñ'},
            {0x98, 'Ñ'},
            {0x99, '¡'},
            {0x9A, '¿'},
            {0x9C, 'ù'},
            {0x9D, 'ì'},
            {0x9E, 'ò'},

            {0xA2, '+'},

            {0xFE, '\n'},

            {0xFF, ' '},
        };

        private static readonly Dictionary<Char, Byte> ToByte = ToChar.Reverse();
        private static Boolean _isRealCodepage = false;

        public static void TryReadFromExecutable(String executablePath)
        {
            const Int32 offset = 0xA4430;
            const Int32 size = 512 - 6; // 0x01 ~ 0xFD
            
            if (_isRealCodepage)
                return;

            if (!File.Exists(executablePath))
                return;

            Encoding win1252 = Encoding.GetEncoding(1252);
            Byte[] tmpB = new Byte[1];
            Char[] tmpC = new Char[1];

            Byte[] buff = TryReadCodepage();
            if (buff == null)
                return;

            unsafe
            {
                fixed (Byte* bptr = buff)
                {
                    UInt16* ptr = (UInt16*) bptr;
                    for (Int32 i = 0; i < size / 2; i++)
                    {
                        Byte index = checked((Byte) (i + 1));
                        UInt16 value = ptr[i];
                        if (value == 0)
                            break;

                        if (index < 8)
                        {
                            if (!ToChar.TryGetValue(index, out var old) || old != value)
                                return; // Cannot validata codepage
                        }
                        else if (index < 0xFE && value <= Byte.MaxValue)
                        {
                            tmpB[0] = checked((Byte) value);
                            if (win1252.GetChars(tmpB, 0, 1, tmpC, 0) != 1)
                                throw new NotSupportedException("if (win1252.GetChars(tmpB, 0, 1, tmpC, 0) != 1)");

                            ToChar[index] = tmpC[0];
                        }
                    }
                }
            }

            _isRealCodepage = true;

            Byte[] TryReadCodepage()
            {
                using var input = File.OpenRead(executablePath);

                if (input.Length < offset + size)
                    return null;

                input.Seek(offset, SeekOrigin.Begin);

                Byte[] data = new Byte[size];
                input.EnsureRead(data, 0, size);

                return data;
            }
        }
    }
}