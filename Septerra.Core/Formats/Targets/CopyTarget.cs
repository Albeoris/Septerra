using System;
using System.IO;

namespace Septerra.Core
{
    public sealed class CopyTarget : ITarget
    {
        public static readonly CopyTarget Instance = new CopyTarget();

        public void Write(ArraySegment<Byte> segment, String outputPath, UInt16 expectedVersion)
        {
            using (var output = File.Create(outputPath))
                output.Write(segment.Array, 0, segment.Count);
        }
    }
}