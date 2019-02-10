using System;
using Septerra.Core;

namespace Septerra.Core.AM
{
    public struct AMImageHeader
    {
        /* 00000000 */
        public UInt16 Width;

        /* 00000002 */
        private UInt16 _height;

        /* 00000004 */
        public UInt32 Zero1;

        /* 00000008 */
        public UInt32 Zero2;

        /* 0000000C */
        public UInt32 Zero3;

        /* 00000010 */
        public UInt32 ImageLineIndex;

        /* 00000014 */
        private UInt32 _height32;

        public UInt16 Height
        {
            get
            {
                Asserts.Expected(_height, _height32);
                return _height;
            }
            set
            {
                _height = value;
                _height32 = value;
            }
        }
    }
}