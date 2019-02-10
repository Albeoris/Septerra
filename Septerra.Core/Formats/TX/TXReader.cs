using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Septerra.Core
{
    public static class TXWriter
    {
        public static Byte[] ToByteArray(IReadOnlyList<TXString> lines)
        {
            Int32 size = GetSize(lines);
            Byte[] array = new Byte[size];

            using (var ms = new MemoryStream(array))
                WriteStrings(ms, lines);

            return array;
        }

        public static void WriteStrings(Stream output, IReadOnlyList<TXString> lines)
        {
            unsafe
            {
                Int32 size = GetSize(lines);
                if (output.CanSeek)
                {
                    Int64 diff = size - (output.Length - output.Position);
                    if (diff > 0)
                        output.SetLength(output.Length + diff);
                }

                UInt32 magicNumber = TXHeader.KnownMagicNumber;
                Int32 count = lines.Count;
                Int32 headerSize = count * sizeof(TXEntry);
                Int32 dataSize = size - sizeof(TXHeader) - headerSize;

                BinaryWriter bw = new BinaryWriter(output);
                
                bw.Write(magicNumber);
                bw.Write(count);
                bw.Write(dataSize);

                Byte[] header = new Byte[headerSize];
                Byte[] data = new Byte[dataSize];

                fixed (Byte* entriesPtr = header)
                fixed (Byte* dataPtr = data)
                {
                    TXEntry* entries = (TXEntry*)entriesPtr;
                    SByte* str = (SByte*)dataPtr;

                    UInt32 offset = 0;

                    Int32 index = 0;
                    foreach (TXString entry in lines)
                    {
                        TXEntry* info = entries + index++;
                        info->Index = entry.Index;
                        info->Offset = checked((UInt32)(offset + headerSize + sizeof(TXHeader)));
                        info->Size = entry.Data.Length + 1;

                        foreach (Char ch in entry.Data)
                            str[offset++] = (SByte)TXEncoding.ToText(ch);

                        str[offset++] = 0;
                    }

                    if (offset != dataSize)
                        throw new InvalidDataException("Something went wrong.");
                }

                output.Write(header, 0, header.Length);
                output.Write(data, 0, data.Length);
            }
        }

        private static Int32 GetSize(IReadOnlyList<TXString> lines)
        {
            unsafe
            {
                return sizeof(TXHeader) + lines.Count * sizeof(TXEntry) + lines.Sum(l => l.Data.Length + 1);
            }
        }
    }

    public sealed class TXString
    {
        public readonly Int32 Index;
        public readonly String Data;

        public TXString(Int32 index, String data)
        {
            Index = index;
            Data = data;
        }

        public override String ToString() => $"{Index}: {Data}";
    }

    public static class TXReader
    {
        public static IReadOnlyList<TXString> ReadStrings(Stream stream)
        {
            unsafe
            {
                TXHeader header = stream.ReadStruct<TXHeader>();
                header.Check();

                Int32 count = header.Count;
                Int32 dataSize = header.DataSize;

                Int32 headerSize = count * sizeof(TXEntry);
                Byte[] entriesHeader = stream.ReadBytes(headerSize);
                Byte[] data = stream.ReadBytes(dataSize);

                List<TXString> result = new List<TXString>(count);

                fixed (Byte* entriesPtr = entriesHeader)
                fixed (Byte* dataPtr = data)
                {
                    TXEntry* entries = (TXEntry*)entriesPtr;
                    SByte* str = (SByte*)dataPtr;

                    for (Int32 i = 0; i < count; i++)
                    {
                        TXEntry entry = entries[i];
                        Int32 offset = checked((Int32)(entry.Offset - headerSize - sizeof(TXHeader)));
                        Int32 size = 0;

                        for (Int32 c = 0; c < entry.Size; c++)
                        {
                            SByte ch = str[offset + c];
                            if (ch == 0)
                                break;

                            str[offset + c] = (SByte)TXEncoding.ToText((Byte)ch);
                            size++;
                        }

                        String line = new String(str, offset, size);
                        result.Add(new TXString(entry.Index, line));
                    }

                    return result;
                }
            }
        }
    }
}