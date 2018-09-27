using System;

namespace Septera
{
    public unsafe struct CHTextureNames
    {
        private fixed SByte _name[16]; // => 0x10
        private fixed SByte _type[24]; // => 0x28

        public String Name
        {
            get
            {
                fixed (SByte* ptr = _name)
                    return new String(ptr);
            }
        }

        public String Type
        {
            get
            {
                fixed (SByte* ptr = _type)
                    return new String(ptr);
            }
        }
    }
}