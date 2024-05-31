using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Septerra.Core
{
    public static class TXEncoding
    {
        public static Char ToText(Byte bt)
        {
            if (ToChar.TryGetValue(bt, out Char value))
                return value;

            if (_isRealCodepage)
                return (Char)0xFF; // It's valid game logic

            throw new NotSupportedException($"[{bt:X2}]");
        }

        public static Byte ToText(Char bt)
        {
            if (ToByte.TryGetValue(bt, out Byte value))
                return value;

            throw new NotSupportedException($"[{checked((Byte) bt):X2}]");
        }

        private static Boolean _isRealCodepage = false;

        public static void TryReadFromExecutable(String executablePath, Boolean cyrillic)
        {
            const Int32 offset = 0xA4430;
            const Int32 size = 512 - 6; // 0x01 ~ 0xFD
            
            if (_isRealCodepage)
                return;

            if (!File.Exists(executablePath))
                return;

            Char[] encoding = GetCharset(Encoding.GetEncoding(1252));
            Byte[] gameCodepage = TryReadCodepage();

            if (gameCodepage == null)
                return;

            unsafe
            {
                fixed (Byte* bptr = gameCodepage)
                {
                    UInt16* ptr = (UInt16*) bptr;
                    for (Int32 i = 0; i < size / 2; i++)
                    {
                        Byte index = checked((Byte) (i + 1));
                        UInt16 ascii = ptr[i];
                        if (ascii == 0)
                            break;

                        if (index < 8)
                        {
                            if (!ToChar.TryGetValue(index, out var old) || old != ascii)
                                return; // Cannot validata codepage
                        }
                        else if (index < 0xFE && ascii <= Byte.MaxValue)
                        {
                            Char ch = encoding[checked((Byte)ascii)];
                            ToChar[index] = ch;
                        }
                    }
                }
            }

            ApplyExternalEncoding(executablePath, cyrillic);
            ToByte = ToChar.Reverse();
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

        private static void ApplyExternalEncoding(String executablePath, Boolean cyrillic)
        {
            String codepagePath = executablePath + ".codepage";
            if (cyrillic)
            {
                TXByteToCharMapping external = new TXByteToCharMapping(GoodOldCyrillicEncoding);
                external.Apply(ToChar);

                if (!File.Exists(codepagePath))
                    external.SaveToFile(codepagePath);
            }
            else if (File.Exists(codepagePath))
            {
                TXByteToCharMapping external = new TXByteToCharMapping();
                external.LoadFromFile(codepagePath);
                external.Apply(ToChar);
            }
        }

        private static Char[] GetCharset(Encoding encoding)
        {
            Byte[] values = new Byte[256];
            for (Int32 i = 0; i < values.Length; i++)
                values[i] = (Byte)i;
            
            return encoding.GetChars(values);
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
        
        private static readonly Dictionary<Byte, Char> GoodOldCyrillicEncoding = new Dictionary<Byte, Char>() 
        {
            {0x60, 'А'},
            {0x61, 'Б'},
            {0x62, 'В'},
            {0x63, 'Г'},
            {0x64, 'Д'},
            {0x65, 'Е'},
            {0x66, 'Ж'},
            {0x67, 'З'},
            {0x68, 'И'},
            {0x69, 'Й'},
            {0x6A, 'К'},
            {0x6B, 'Л'},
            {0x6C, 'М'},
            {0x6D, 'Н'},
            {0x6E, 'О'},
            {0x6F, 'П'},

            {0x70, 'Р'},
            {0x71, 'С'},
            {0x72, 'Т'},
            {0x73, 'У'},
            {0x74, 'Ф'},
            {0x75, 'Х'},
            {0x76, 'Ц'},
            {0x77, 'Ч'},
            {0x78, 'Ш'},
            {0x79, 'Щ'},
            {0x7A, 'Ъ'},
            {0x7B, 'Ы'},
            {0x7C, 'Ь'},
            {0x7D, 'Э'},
            {0x7E, 'Ю'},
            {0x7F, 'Я'},

            {0x80, 'а'},
            {0x81, 'б'},
            {0x82, 'в'},
            {0x83, 'г'},
            {0x84, 'д'},
            {0x85, 'е'},
            {0x86, 'ж'},
            {0x87, 'з'},
            {0x88, 'и'},
            {0x89, 'й'},
            {0x8A, 'к'},
            {0x8B, 'л'},
            {0x8C, 'м'},
            {0x8D, 'н'},
            {0x8E, 'о'},
            {0x8F, 'п'},

            {0x90, 'р'},
            {0x91, 'с'},
            {0x92, 'т'},
            {0x93, 'у'},
            {0x94, 'ф'},
            {0x95, 'х'},
            {0x96, 'ц'},
            {0x97, 'ч'},
            {0x98, 'ш'},
            {0x99, 'щ'},
            {0x9A, 'ъ'},
            {0x9B, 'ы'},
            {0x9C, 'ь'},
            {0x9D, 'э'},
            {0x9E, 'ю'},
            {0x9F, 'я'},
        };
        
        private static Dictionary<Char, Byte> ToByte = ToChar.Reverse();
    }
}