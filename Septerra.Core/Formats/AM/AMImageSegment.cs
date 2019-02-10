using System;

namespace Septerra.Core.AM
{
    public struct AMImageSegment
    {
        private UInt16 _pageOffset;
        private UInt16 _pageNumber;

        public UInt16 LeftPadding;
        public UInt16 SizeInBytes;

        public Int32 Offset
        {
            get => 65536 * _pageNumber + _pageOffset;
            set
            {
                _pageNumber = checked((UInt16)(value / 65536));
                _pageOffset = checked((UInt16)(value % 65536));
            }
        }
    }
}