using System;
using System.IO;

namespace Septerra.Core
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

            for (Int32 i = characterCount - 1; i >= 0; i--)
            {
                Int32 value = self.ReadByte();
                if (value < 0x30 || value > 0x39)
                    throw new InvalidDataException($"Unexpected value: 0x{value:X2} has occurred. Expected value in range [0x{0x30:X2}...0x{0x39:X2}].");

                Int32 number = (value - 0x30);
                for (Int32 k = 0; k < i; k++)
                    number *= 10;
                
                result += number;
            }

            return result;
        }

        public static T ReadStruct<T>(this Stream input) where T : unmanaged
        {
            return ReadStructs<T>(input, count: 1)[0];
        }

        public static T[] ReadStructs<T>(this Stream input, Int32 count) where T : unmanaged
        {
            if (count < 1)
                return new T[0];

            unsafe
            {
                Array result = new T[count];
                Int32 entrySize = UnsafeTypeCache<T>.UnsafeSize;
                using (UnsafeTypeCache<Byte>.ChangeArrayType(result, entrySize))
                    input.EnsureRead((Byte[])result, 0, result.Length);
                return (T[])result;
            }
        }

        public static void WriteStruct<T>(this Stream input, T value) where T : unmanaged 
        {
            unsafe
            {
                Byte[] buff = new Byte[UnsafeTypeCache<T>.UnsafeSize];

                fixed (Byte* buffPtr = buff)
                    *((T*)buffPtr) = value;

                input.Write(buff, 0, buff.Length);
            }
        }

        public static void WriteStructs<T>(this Stream input, T[] value) where T : unmanaged
        {
            WriteStructs(input, value, 0, value.Length);
        }

        public static void WriteStructs<T>(this Stream input, ArraySegment<T> value) where T : unmanaged
        {
            WriteStructs(input, value.Array, value.Offset, value.Count);
        }

        public static void WriteStructs<T>(this Stream input, T[] value, Int32 offset, Int32 count) where T : unmanaged
        {
            if (Asserts.InRange(count, 0, value.Length-offset) < 1)
                return;

            unsafe
            {
                Int32 entrySize = UnsafeTypeCache<T>.UnsafeSize;
                using (UnsafeTypeCache<Byte>.ChangeArrayType(value, entrySize))
                    input.Write((Byte[])(Object)value, offset * entrySize, count * entrySize);
            }
        }

        public static Byte[] ReadToEnd(this Stream input)
        {
            Int32 size = checked((Int32)(input.Length - input.Position));
            return ReadBytes(input, size);
        }
    }
}