using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using static Septera.Asserts;

namespace Septera
{
    public sealed class GVReader
    {
        public const UInt32 MagicNumber = 0x5647; // GV

        private readonly Stream _stream;
        private readonly BinaryReader _br;

        public GVReader(Stream stream, UInt16 expectedVersion)
        {
            _stream = stream;
            _br = new BinaryReader(stream, Encoding.ASCII, leaveOpen: true);

            Expected(_br.ReadUInt16(), MagicNumber);
            Expected(_stream.ParseAsciiNumber(characterCount: 2), expectedVersion);
        }

        public IReadOnlyCollection<String> ReadLines()
        {
            unsafe
            {
                Int32 headerOffset = Positive(_br.ReadInt32());
                Int32 headerCount = Positive(_br.ReadInt32());
                Int32 headerSize = checked(headerCount * sizeof(GVHeaderEntry));

                Int32 contentOffset = Positive(_br.ReadInt32());
                Int32 contentSize = Positive(_br.ReadInt32());

                _stream.SetPosition(headerOffset);
                Byte[] header = _stream.ReadBytes(headerSize);

                _stream.SetPosition(contentOffset);
                Byte[] content = _stream.ReadBytes(contentSize);

                String[] result = new String[headerCount];

                fixed (Byte* headerBytes = header)
                fixed (Byte* contentBytes = content)
                {
                    GVHeaderEntry* entriesPtr = (GVHeaderEntry*)headerBytes;
                    SByte* contentPtr = (SByte*)contentBytes;

                    for (Int32 i = 0; i < headerCount; i++)
                    {
                        if (IsEmpty(entriesPtr, i))
                        {
                            result[i] = $"{i:D4}|";
                            continue;
                        }

                        GVHeaderEntry entry = entriesPtr[i];

                        CheckEntry(i, ref entry, contentPtr);

                        Int32 offset = InRange(entry.Offset, minValue: 0, maxValue: contentSize - 1);
                        Int32 size = InRange(entry.Size, minValue: 2, maxValue: contentSize - offset); // \0 terminated strings
                        UInt32 value = entry.Value;
                        UInt32 mask = entry.Mask;

                        String name = new String(contentPtr, offset, size - 1, Encoding.ASCII);

                        if ((value & mask) != value)
                            throw new InvalidDataException($"Value 0x[{value:X8}] of the variable [{i:D4}:{name}] is out of mask [{mask:X8}].");

                        FormatTypeAndValue(mask, value, out var formattedType, out var formattedValue);
                        result[i] = $"{i:D4}| {formattedType} {name} = {formattedValue}";
                    }
                }

                return result;
            }
        }

        private static void FormatTypeAndValue(UInt32 mask, UInt32 value, out String displayType, out String displayValue)
        {
            switch (mask)
            {
                case 0x01:
                    displayType = "BOOL";
                    displayValue = value == 0 ? "false" : "true";
                    return;
                case 0xFF:
                    displayType = "INT8";
                    break;
                case 0xFFFF:
                    displayType = "INT16";
                    break;
                case 0xFFFFFFFF:
                    displayType = "INT32";
                    break;
                default:
                    throw new NotSupportedException($"{mask:X8}");
            }

            displayValue = value.ToString(CultureInfo.InvariantCulture);
        }

        private static unsafe Boolean IsEmpty(GVHeaderEntry* entriesPtr, Int32 i)
        {
            Int64* raw = (Int64*)(entriesPtr + i);
            return raw[0] == 0 && raw[1] == 0;
        }

        private static unsafe void CheckEntry(Int32 i, ref GVHeaderEntry entry, SByte* contentPtr)
        {
            for (Int32 r = 0; r < 2; r++)
            {
                Int32 lastCharacter = entry.Offset + entry.Size - 1;

                for (int k = entry.Offset; k < lastCharacter; k++)
                {
                    if (contentPtr[k] == 0x00)
                        goto check;
                }

                if (contentPtr[lastCharacter] != 0x00)
                    goto check;

                return;

                @check:
                if (TryCorrectEntry(i, ref entry))
                    continue;

                throw new NotSupportedException($"Unexcepted character: 0x{contentPtr[lastCharacter]:X2} at position {lastCharacter}. Expected: 0x00");
            }
        }

        private static Boolean TryCorrectEntry(Int32 i, ref GVHeaderEntry entry)
        {
            if (i == 163 && entry.Offset == 2078 && entry.Size == 12)
            {
                entry.Size = 15;
                return true;
            }

            if (i == 164 && entry.Offset == 2090 && entry.Size == 15)
            {
                entry.Offset = 2093;
                entry.Size = 12;
                return true;
            }

            if (i == 359 && entry.Offset == 4554 && entry.Size == 12)
            {
                entry.Size = 17;
                return true;
            }

            if (i == 360 && entry.Offset == 4566 && entry.Size == 17)
            {
                entry.Offset = 4571;
                entry.Size = 12;
                return true;
            }
            
            if (i == 522 && entry.Offset == 6544 && entry.Size == 12)
            {
                entry.Size = 11;
                return true;
            }

            if (i == 523 && entry.Offset == 6556 && entry.Size == 11)
            {
                entry.Offset = 6555;
                return true;
            }
            
            if (i == 524 && entry.Offset == 6567 && entry.Size == 11)
            {
                entry.Offset = 6566;
                entry.Size = 13;
                return true;
            }

            if (i == 525 && entry.Offset == 6578 && entry.Size == 13)
            {
                entry.Offset = 6579;
                return true;
            }

            if (i == 526 && entry.Offset == 6591 && entry.Size == 13)
            {
                entry.Offset = 6592;
                return true;
            }

            if (i == 527 && entry.Offset == 6604 && entry.Size == 13)
            {
                entry.Offset = 6605;
                return true;
            }

            if (i == 528 && entry.Offset == 6617 && entry.Size == 13)
            {
                entry.Offset = 6618;
                entry.Size = 11;
                return true;
            }

            if (i == 529 && entry.Offset == 6630 && entry.Size == 11)
            {
                entry.Offset = 6629;
                return true;
            }

            if (i == 738 && entry.Offset == 8774 && entry.Size == 11)
            {
                entry.Size = 10;
                return true;
            }

            if (i == 739 && entry.Offset == 8785 && entry.Size == 10)
            {
                entry.Offset = 8784;
                return true;
            }

            if (i == 740 && entry.Offset == 8795 && entry.Size == 10)
            {
                entry.Offset = 8794;
                return true;
            }

            if (i == 741 && entry.Offset == 8805 && entry.Size == 10)
            {
                entry.Offset = 8804;
                return true;
            }

            if (i == 743 && entry.Offset == 8825 && entry.Size == 10)
            {
                entry.Size = 11;
                return true;
            }

            if (i == 744 && entry.Offset == 8835 && entry.Size == 11)
            {
                entry.Offset = 8836;
                entry.Size = 10;
                return true;
            }

            return false;
        }
    }
}