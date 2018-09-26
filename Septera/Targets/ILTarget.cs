using System;
using System.IO;

namespace Septera
{
    public sealed class ILTarget : ITarget
    {
        public static readonly ILTarget Instance = new ILTarget();

        public void Write(ArraySegment<Byte> segment, String outputPath, UInt16 expectedVersion)
        {
            using (MemoryStream sourceFile = new MemoryStream(segment.Array, 0, segment.Count))
            using (StreamWriter outputFile = File.CreateText(outputPath))
            {
                ILReader reader = new ILReader(sourceFile, expectedVersion);
                String[][] lines = reader.ReadLines();

                for (var i = 0; i < lines.Length; i++)
                {
                    outputFile.WriteLine($"{i:D4}:");

                    String[] entries = lines[i];
                    foreach (String line in entries)
                        outputFile.WriteLine(line);

                    outputFile.WriteLine();
                }
            }
        }
    }
}