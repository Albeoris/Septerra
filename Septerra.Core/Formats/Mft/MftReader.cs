using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Septerra.Core
{
    public sealed class MftReader
    {
        public const String MftFileName = "septerra.mft";
        public const String IdxFileName = "septerra.idx";

        public static MftReader Create(String gameFolder)
        {
            var filePath = Path.Combine(gameFolder, MftFileName);
            return new MftReader(filePath);
        }

        private readonly String _filePath;
        private readonly String _gameFolder;

        public MftReader(String filePath)
        {
            _filePath = filePath;
            _gameFolder = Path.GetDirectoryName(filePath);
        }

        public MftContent ReadContent()
        {
            using (StreamReader sr = new StreamReader(_filePath, Encoding.ASCII))
            {
                TerrabuilderVersion version = ReadVersion(sr);
                String indexFilePath = ReadIndexFilePath(sr);

                List<DbPackage> packages = new List<DbPackage>();
                while (TryReadPackage(sr, packages.Count, out DbPackage package))
                    packages.Add(package);

                return new MftContent(version, indexFilePath, packages.ToArray());
            }
        }

        private static TerrabuilderVersion ReadVersion(StreamReader sr)
        {
            return new TerrabuilderVersion(sr.ReadLine());
        }

        private String ReadIndexFilePath(StreamReader sr)
        {
            String relativePath = sr.ReadLine();
            if (relativePath == null)
                throw new EndOfStreamException($"Unexpected end of file [{_filePath}].");

            if (!relativePath.EndsWith(IdxFileName, StringComparison.OrdinalIgnoreCase))
                throw new NotSupportedException($"An unexpected entry [{relativePath}] occurred in the file [{_filePath}]. Expected: [LOCAL]\\{IdxFileName}");

            return ToFullPath(relativePath);
        }

        private Boolean TryReadPackage(StreamReader sr, Int32 index, out DbPackage dbPackage)
        {
            while (!sr.EndOfStream)
            {
                String relativePath = sr.ReadLine();
                if (String.IsNullOrWhiteSpace(relativePath))
                    continue;

                String fullPath = ToFullPath(relativePath);
                String name = Path.GetFileNameWithoutExtension(fullPath);
                String extension = Path.GetExtension(fullPath);
                if (extension == null || !extension.Equals(".db", StringComparison.OrdinalIgnoreCase))
                    throw new NotSupportedException($"An unexpected entry [{relativePath}] occurred in the file [{_filePath}]. Expected: *.db");

                dbPackage = new DbPackage(index, name, fullPath);
                return true;
            }

            dbPackage = null;
            return false;

        }

        private String ToFullPath(String relativePath)
        {
            String fullPath = relativePath.Replace("[LOCAL]", _gameFolder);
            
            if (fullPath == relativePath)
                fullPath = Path.GetFullPath(relativePath);
         
            return fullPath;
        }
    }
}