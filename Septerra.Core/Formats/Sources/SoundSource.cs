using System;
using System.IO;

namespace Septerra.Core.Sources
{
    public sealed class SoundSource
    {
        public static readonly SoundSource Instance = new SoundSource();

        public Byte[] Read(String inputFile)
        {
            using (var input = File.OpenRead(inputFile))
            {
                return Read(input);
            }
        }

        public Byte[] Read(FileStream input)
        {
            Byte[] result = new Byte[input.Length + 4];

            result[0] = 0x56; // V
            result[1] = 0x53; // S
            result[2] = 0x53; // S
            result[3] = 0x46; // F

            input.EnsureRead(result, 4, (Int32)input.Length);

            return result;
        }
    }
}