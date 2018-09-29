using System;
using System.IO;
using System.Text;

namespace Septera
{
    public sealed class CHReader
    {
        public const UInt32 MagicNumber = 0x4843; // CH

        private readonly Stream _stream;
        private readonly BinaryReader _br;

        public CHReader(Stream stream, UInt16 expectedVersion)
        {
            _stream = stream;
            _br = new BinaryReader(stream, Encoding.ASCII, leaveOpen: true);

            Asserts.Expected(_br.ReadUInt16(), MagicNumber);
            Asserts.Expected(_stream.ParseAsciiNumber(characterCount: 2), expectedVersion);
        }

        public CHSegment1[] Read()
        {
            CHHeader header = _stream.ReadStruct<CHHeader>();

            CHSegment1[] segment1 = ReadSegment<CHSegment1>(header.Offset1);
            CHSegment2[] segment2 = ReadSegment<CHSegment2>(header.Offset2);
            CHSegment3[] segment3 = ReadSegment<CHSegment3>(header.Offset3);
            CHTextureNames[] textureNames = ReadSegment<CHTextureNames>(header.TextureNames);
            CHSegment5[] segment5 = ReadSegment<CHSegment5>(header.Offset5);
            CHSegment6[] segment6 = ReadSegment<CHSegment6>(header.Offset6);
            CHSegment7[] segment7 = ReadSegment<CHSegment7>(header.Offset7);
            CHSegment8[] segment8 = ReadSegment<CHSegment8>(header.Offset8);
            CHSegmentSkillName[] segmentSkillName = ReadSegment<CHSegmentSkillName>(header.SkillNames);
            CHSegment10[] segment10 = ReadSegment<CHSegment10>(header.Offset10);
            CHSegment11[] segment11 = ReadSegment<CHSegment11>(header.Offset11);
            CHSegment12[] segment12 = ReadSegment<CHSegment12>(header.Offset12);
            Byte[] segment13 = ReadSegment<Byte>(header.Offset13);

            return segment1;
        }

        private T[] ReadSegment<T>(CHSegmentHeader segmentHeader) where T : struct
        {
            if (segmentHeader.Count == 0)
                return new T[0];

            _stream.SetPosition(segmentHeader.Offset);
            return _stream.ReadStructs<T>(segmentHeader.Count);
        }
    }
}