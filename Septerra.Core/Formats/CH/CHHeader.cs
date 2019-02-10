using System;

namespace Septerra.Core
{
    public struct CHHeader
    {
        public Int16 Unknown1;
        public Int16 Unknown2;

        public CHSegmentHeader Offset1;
        public CHSegmentHeader Offset2;
        public CHSegmentHeader Offset3;
        public CHSegmentHeader TextureNames;
        public CHSegmentHeader Offset5;
        public CHSegmentHeader Offset6;
        public CHSegmentHeader Offset7;
        public CHSegmentHeader Offset8;
        public CHSegmentHeader SkillNames;
        public CHSegmentHeader Offset10;
        public CHSegmentHeader Offset11;
        public CHSegmentHeader Offset12;
        public CHSegmentHeader Offset13;
    }
}