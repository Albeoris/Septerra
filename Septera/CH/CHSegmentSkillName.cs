using System;

namespace Septera
{
    public unsafe struct CHSegmentSkillName
    {
        private fixed SByte _name[32]; // => 0x20

        public String Name
        {
            get
            {
                fixed (SByte* ptr = _name)
                    return new String(ptr);
            }
        }
    }
}