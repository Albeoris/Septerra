using System;
using System.IO;

namespace Septerra.Core
{
    public sealed class SoundTarget : ITarget
    {
        public static readonly SoundTarget Instance = new SoundTarget();

        public void Write(ArraySegment<Byte> segment, String outputPath, UInt16 expectedVersion)
        {
            Int32 offset = segment.Offset;
            var array = segment.Array ?? throw new ArgumentNullException(nameof(segment));
            if (segment.Count < 4) throw new ArgumentException(nameof(segment));

            Asserts.Expected(array[offset + 0], 0x56); // V
            Asserts.Expected(array[offset + 1], 0x53); // S
            Asserts.Expected(array[offset + 2], 0x53); // S
            Asserts.Expected(array[offset + 3], 0x46); // F

            using (var output = File.Create(outputPath))
                output.Write(array, offset + 4, segment.Count - 4 - offset);
        }
    }
}