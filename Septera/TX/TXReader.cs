using System;
using System.IO;
using System.Text;

namespace Septera
{
    public sealed class TXReader
    {
        public const UInt32 MagicNumber = 0x30305854;

        private readonly Encoding _encoding;
        private readonly BinaryReader _br;

        public TXReader(Stream stream)
        {
            _encoding = Encoding.ASCII;
            _br = new BinaryReader(stream, _encoding, leaveOpen: true);

            if (_br.ReadUInt32() != MagicNumber)
                throw new InvalidDataException();
        }

        public Byte[][] ReadLines()
        {
            unsafe
            {
                var count = _br.ReadInt32();
                var dataSize = _br.ReadInt32();

                var headerSize = count * sizeof(TXEntry);
                var header = _br.ReadBytes(headerSize);

                Byte[] buff = new Byte[4096];

                fixed (Byte* entriesPtr = header)
                {
                    TXEntry* entries = (TXEntry*)entriesPtr;

                    String[] lines = new String[count];
                    Byte[][] bytes = new Byte[count][];
                    for (int i = 0; i < count; i++)
                    {
                        var entry = entries[i];

                        if (_br.BaseStream.Position != entry.Offset)
                            _br.BaseStream.Position = entry.Offset;

                        if (entry.Size > buff.Length)
                            buff = new Byte[entry.Size];

                        var data = _br.ReadBytes(entry.Size);
                        if (data[data.Length - 1] == 0)
                        {
                            Array.Resize(ref data, data.Length - 1);
                        }
                        else
                        {
                            //
                        }

                        //if (_br.BaseStream.Read(buff, 0, entry.Size) != entry.Size)
                        //    throw new InvalidDataException();
                        //
                        //lines[i] = _encoding.GetString(buff, 0, entry.Size - 1);

                        bytes[i] = data;
                    }

                    return bytes;
                }
            }
        }
    }
}