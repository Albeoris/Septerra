using System;
using System.Collections.Generic;
using System.IO;

namespace Septera
{
    public sealed class GVTarget : ITarget
    {
        public static readonly GVTarget Instance = new GVTarget();

        public void Write(ArraySegment<Byte> segment, String outputPath, UInt16 expectedVersion)
        {
            using (MemoryStream sourceFile = new MemoryStream(segment.Array, 0, segment.Count))
            {
                GVReader reader = new GVReader(sourceFile, expectedVersion);
                IReadOnlyCollection<String> lines = reader.ReadLines();

                File.WriteAllLines(outputPath, lines);
            }
        }
    }
}