using System;
using System.IO;
using System.Text;

namespace Septera
{
    public sealed class TextTarget : ITarget
    {
        public static readonly TextTarget Instance = new TextTarget();

        public void Write(ArraySegment<Byte> segment, String outputPath, UInt16 expectedVersion)
        {
            StringBuilder sb = new StringBuilder(4096);

            using (MemoryStream sourceFile = new MemoryStream(segment.Array, 0, segment.Count))
            using (FileStream targetFile = File.Create(outputPath))
            using (StreamWriter targetSw = new StreamWriter(targetFile, Encoding.ASCII))
            {
                TXReader reader = new TXReader(sourceFile);
                Byte[][] lines = reader.ReadLines();

                for (var i = 0; i < lines.Length; i++)
                {
                    sb.Clear();
                    sb.Append($"{i:D3}|");
                    var line = lines[i];

                    foreach (var bt in line)
                        sb.Append(TXEncoding.ToText(bt));
                    targetSw.WriteLine(sb);
                }
            }
        }
    }
}