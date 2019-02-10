using System;

namespace Septerra.Core
{
    public interface ITarget
    {
        void Write(ArraySegment<Byte> segment, String outputPath, UInt16 expectedVersion);
    }
}