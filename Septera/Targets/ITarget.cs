using System;

namespace Septera
{
    public interface ITarget
    {
        void Write(ArraySegment<Byte> segment, String outputPath, UInt16 expectedVersion);
    }
}