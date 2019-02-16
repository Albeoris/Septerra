using System;
using System.Collections.Generic;
using System.IO;
using Septerra.Core;
using Septerra.Core.Sources;

namespace Septerra.Core.DB
{
    public static class NativeFormatProvider
    {
        public static readonly TiffImageReader _tiffReader = new TiffImageReader();

        public static Byte[] ReadAllBytes(String resourcePath)
        {
            using (FileStream input = File.OpenRead(resourcePath))
                return ReadAllBytes(resourcePath, input);
        }

        public static Byte[] ReadAllBytes(String resourcePath, FileStream input)
        {
            switch (Path.GetExtension(resourcePath))
            {
                case ".txt":
                    return ReadTextFile(resourcePath, input);
                case ".tiff":
                    return ReadImageFile(resourcePath, input);
                case ".mp3":
                    return ReadMp3File(resourcePath, input);
                default:
                    return input.ReadToEnd();
            }
        }

        private static Byte[] ReadMp3File(String resourcePath, FileStream input)
        {
            return SoundSource.Instance.Read(input);
        }

        private static Byte[] ReadImageFile(String resourcePath, FileStream input)
        {
            UnsafeList<Byte> uns = _tiffReader.Read(resourcePath, input);
            return uns.CopyToArray();
        }

        private static Byte[] ReadTextFile(String resourcePath, FileStream input)
        {
            StreamReader sr = new StreamReader(input);
            IReadOnlyList<TXString> lines = TextSource.ReadStrings(sr);
            return TXWriter.ToByteArray(lines);
        }
    }
}