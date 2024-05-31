using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Septerra.Core
{
    public sealed class TextTarget : ITarget
    {
        public static readonly TextTarget Instance = new TextTarget();

        public void Write(ArraySegment<Byte> segment, String outputPath, UInt16 expectedVersion)
        {
            using (MemoryStream sourceFile = new MemoryStream(segment.Array, 0, segment.Count))
            using (FileStream targetFile = File.Create(outputPath))
            using (StreamWriter targetSw = new StreamWriter(targetFile, Encoding.UTF8))
            {
                IReadOnlyList<TXString> lines = TXReader.ReadStrings(sourceFile);
                WriteStrings(lines, targetSw);
            }
        }

        public static void WriteStrings(IReadOnlyList<TXString> lines, StreamWriter targetSw)
        {
            foreach (TXString line in lines)
            {
                String[] parts = line.Data.Split('\n');

                targetSw.Write($"{line.Index:D5}|");
                if (parts.Length > 0)
                    targetSw.WriteLine(parts[0]);

                for (Int32 i = 1; i < parts.Length; i++)
                {
                    targetSw.Write("XXXXX|");
                    targetSw.WriteLine(parts[i]);
                }
            }
        }
    }
}