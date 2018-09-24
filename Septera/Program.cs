using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SepteraNative;

namespace Septera
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            List<String> packages = new List<String>(18);
            List<Int32> packageFiles = new List<Int32>(18);

            using (var mftFile = File.OpenRead("septerra.mft"))
            using (var mft = new StreamReader(mftFile))
            {
                //Assert(mft.ReadLine(), "Terrabuilder version 0.9802 LV25 AM04 GV00 TX00 CH14 IL00 ");
                mft.ReadLine();
                Assert(mft.ReadLine(), "[LOCAL]\\septerra.idx");

                while (!mft.EndOfStream)
                {
                    String line = mft.ReadLine();
                    if (String.IsNullOrEmpty(line))
                        continue;

                    Assert(line, str => str.EndsWith(".db"));
                    line = line.Replace("[LOCAL]", Environment.CurrentDirectory);

                    packages.Add(line);
                    packageFiles.Add(0);
                }
            }

            using (var idxFile = File.OpenRead("septerra.idx"))
            using (var idx = new BinaryReader(idxFile))
            {
                Int64 fileNumber = idxFile.Length;
                fileNumber /= 0x20;

                for (int i = 0; i < fileNumber; i++)
                {
                    var dummy = idx.ReadInt32();
                    var pck = idx.ReadInt32();
                    var offset = idx.ReadInt32();
                    var size = idx.ReadInt32();
                    var type = idx.ReadInt32();
                    var zsize = idx.ReadInt32();
                    var tstamp = idx.ReadInt64();

                    packageFiles[pck]++;
                }
            }

            String _currentPackage = null;
            FileStream _package = null;

            using (var idxFile = File.OpenRead("septerra.idx"))
            using (var idx = new BinaryReader(idxFile))
            {
                Int64 fileNumber = idxFile.Length;
                fileNumber /= 0x20;

                Byte[] buff = new Byte[80000];

                for (int i = 0; i < fileNumber; i++)
                {
                    var dummy = idx.ReadInt32();
                    var pck = idx.ReadInt32();
                    var offset = idx.ReadInt32();
                    var size = idx.ReadInt32();
                    var type = idx.ReadInt32();
                    var zsize = idx.ReadInt32();
                    var tstamp = idx.ReadInt64();

                    var compressionType = type & 0xFF; // 0 or 1

                    String package = packages[pck];
                    var fileName = Path.GetFileNameWithoutExtension(package);
                    String outputDirectory;
                    if (packageFiles[pck] == 1)
                        outputDirectory = Path.Combine(Environment.CurrentDirectory, "Output");
                    else
                        outputDirectory = Path.Combine(Environment.CurrentDirectory, "Output", fileName);
                    var outputFile = Path.Combine(outputDirectory, $"{fileName}_{i:D3}.{type}");
                    //if (zsize != size)
                    //    outputFile += ".compressed";

                    Directory.CreateDirectory(outputDirectory);

                    if (_currentPackage != package)
                    {
                        if (_package != null)
                        {
                            _package.Dispose();
                            _package = null;
                        }

                        _package = File.OpenRead(package);
                        _currentPackage = package;
                    }

                    if (_package.Position != offset)
                        _package.Position = offset;

                    Boolean isFirst = size == zsize;
                    Boolean isText = false;
                    Boolean isMovie = false;

                    using (var output = File.Create(outputFile))
                    {
                        if (size != zsize)
                        {
                            if (compressionType != 1)
                                throw new NotSupportedException();

                            if (buff.Length < size)
                                Array.Resize(ref buff, size);

                            fixed (byte* buffPtr = buff)
                            {
                                Int32 readedSize = Decompressor.ReadCompressedFile(zsize, buffPtr, _package);
                                if (size != readedSize)
                                    throw new NotSupportedException();

                                CheckFileType(readedSize, buff, ref isText, ref isMovie);
                                output.Write(buff, 0, readedSize);
                                zsize = 0;
                            }
                        }
                        else
                        {
                            while (zsize > 0)
                            {
                                Int32 readed = _package.Read(buff, 0, Math.Min(buff.Length, zsize));
                                if (isFirst)
                                {
                                    CheckFileType(readed, buff, ref isText, ref isMovie);
                                    isFirst = false;
                                }

                                output.Write(buff, 0, readed);

                                zsize -= readed;
                            }
                        }
                    }

                    if (isMovie)
                        Rename(outputFile, Path.ChangeExtension(outputFile, ".qt"));
                    else if (isText)
                        ToText(outputFile, Path.ChangeExtension(outputFile, ".txt"));
                }
            }
        }

        private static void CheckFileType(Int32 readed, Byte[] buff, ref Boolean isText, ref Boolean isMovie)
        {
            if (readed > 8)
            {
                if (BitConverter.ToUInt32(buff, 0) == TXReader.MagicNumber)
                {
                    isText = true;
                }

                if (BitConverter.ToInt32(buff, 4) == 0x766F6F6D)
                {
                    isMovie = true;
                }
            }
        }

        private static void Rename(String source, String target)
        {
            if (source != target)
            {
                File.Delete(target);
                File.Move(source, target);
            }
        }

        private static void ToText(String source, String target)
        {
            StringBuilder sb = new StringBuilder(4096);

            using (var sourceFile = File.OpenRead(source))
            using (var targetFile = File.Create(target))
            using (var targetSw = new StreamWriter(targetFile, Encoding.ASCII))
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
                    //targetSw.WriteLine($"Line {i}:");
                    targetSw.WriteLine(sb);
                }
            }

            File.Delete(source);
        }

        private static T Assert<T>(T value1, T value2)
        {
            if (!value1.Equals(value2))
                throw new InvalidDataException();

            return value1;
        }

        private static T Assert<T>(T value1, Func<T, Boolean> value2)
        {
            if (!value2(value1))
                throw new InvalidDataException();

            return value1;
        }
    }
}