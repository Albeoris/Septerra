using System;
using System.IO;

namespace Septerra
{
    public sealed class UnpackGamePackagesSpecPreprocessor : UnpackGamePackagesSpec
    {
        public String GameDirectoryPath { get; set; }

        public void Preprocess()
        {
            CheckGameDirectoryExists();
            CreateTargetDirectory();
        }

        private void CheckGameDirectoryExists()
        {
            if (GameDirectory == null)
            {
                GameDirectory = new GameDirectoryDescriptor(Path.GetFullPath(GameDirectoryPath));
                if (!GameDirectory.IsMftExists)
                    throw new FileNotFoundException($"Cannot find a game archive file descriptor ({GameDirectory.MftPath}).");
            }
        }

        private void CreateTargetDirectory()
        {
            if (String.IsNullOrEmpty(OutputDirectory))
                OutputDirectory = Path.Combine(GameDirectory.DirectoryPath, "Data");

            OutputDirectory = Path.GetFullPath(OutputDirectory);
            Directory.CreateDirectory(OutputDirectory);
        }
    }
}