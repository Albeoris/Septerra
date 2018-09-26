using System;
using System.IO;

namespace Septera
{
    public static class ExtensionMethodsStream
    {
        public static void SetPosition(this Stream self, Int32 position)
        {
            if (self.Position != position)
                self.Position = position;
        }

        public static void EnsureRead(this Stream self, Byte[] buff, Int32 offset, Int32 size)
        {
            Int32 readed;
            while (size > 0 && (readed = self.Read(buff, offset, size)) != 0)
            {
                size -= readed;
                offset += readed;
            }

            if (size != 0)
                throw new EndOfStreamException("Unexpected end of stream.");
        }

        public static Byte[] ReadBytes(this Stream self, Int32 size)
        {
            Byte[] buffer = new Byte[size];
            EnsureRead(self, buffer, 0, size);
            return buffer;
        }

        public static Int32 ParseAsciiNumber(this Stream self, Int32 characterCount)
        {
            if (characterCount < 1) throw new ArgumentOutOfRangeException(nameof(characterCount), $"Argument size [{characterCount}] must be greater than 0.");

            Int32 result = 0;

            for (Int32 i = characterCount; i > 0; i--)
            {
                Int32 value = self.ReadByte();
                if (value < 0x30 || value > 0x39)
                    throw new InvalidDataException($"Unexpected value: 0x{value:X2} has occurred. Expected value in range [0x{0x30:X2}...0x{0x39:X2}].");

                result += (value - 0x30) * (i * 10);
            }

            return result;
        }
    }
}