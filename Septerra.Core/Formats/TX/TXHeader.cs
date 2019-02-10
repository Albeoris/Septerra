using System;

namespace Septerra.Core
{
    public struct TXHeader
    {
        public const UInt32 KnownMagicNumber = 0x30305854;

        public UInt32 MagicNumber;
        public Int32 Count;
        public Int32 DataSize;

        public void Check()
        {
            Asserts.Expected(MagicNumber, KnownMagicNumber);
        }
    }
}