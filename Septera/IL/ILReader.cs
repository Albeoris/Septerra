using System;
using System.IO;
using System.Text;
using Septera;

public sealed class ILReader
{
    public const UInt32 MagicNumber = 0x4C49; // IL

    private readonly Stream _stream;
    private readonly BinaryReader _br;

    public ILReader(Stream stream, UInt16 expectedVersion)
    {
        _stream = stream;
        _br = new BinaryReader(stream, Encoding.ASCII, leaveOpen: true);

        Asserts.Expected(_br.ReadUInt16(), MagicNumber);
        Asserts.Expected(_stream.ParseAsciiNumber(characterCount: 2), expectedVersion);
    }

    public String[][] ReadLines()
    {
        unsafe
        {
            Int32 headerOffset = Asserts.Positive(_br.ReadInt32());
            Int32 headerCount = Asserts.Positive(_br.ReadInt32());
            Int32 headerSize = checked(headerCount * sizeof(ILHeaderEntry));

            Int32 contentOffset = Asserts.Positive(_br.ReadInt32());
            Int32 contentCount = Asserts.Positive(_br.ReadInt32());
            Int32 contentSize = checked(contentCount * sizeof(ILValueEntry));

            _stream.SetPosition(headerOffset);
            Byte[] header = _stream.ReadBytes(headerSize);

            _stream.SetPosition(contentOffset);
            Byte[] content = _stream.ReadBytes(contentSize);

            String[][] result = new String[headerCount][];

            fixed (Byte* headerBytes = header)
            fixed (Byte* contentBytes = content)
            {
                ILHeaderEntry* entriesPtr = (ILHeaderEntry*)headerBytes;
                ILValueEntry* contentPtr = (ILValueEntry*)contentBytes;

                for (Int32 i = 0; i < headerCount; i++)
                {
                    ILHeaderEntry entry = entriesPtr[i];

                    Int32 index = Asserts.InRange(entry.Index, minValue: 0, maxValue: contentCount - 1);
                    Int32 count = Asserts.InRange(entry.Count, minValue: 0, maxValue: contentCount - index);

                    String[] entryContent = new String[count];
                    result[i] = entryContent;

                    for (Int32 k = 0; k < count; k++)
                    {
                        ILValueEntry value = contentPtr[index + k];
                        entryContent[k] = FormatValue(in value);
                    }
                }
            }

            return result;
        }
    }

    private String FormatValue(in ILValueEntry value)
    {
        return $"0x{value.Unknown1:X8}, 0x{value.Unknown2:X8}, 0x{value.Unknown3:X8}, 0x{value.Unknown4:X8}";
    }
}