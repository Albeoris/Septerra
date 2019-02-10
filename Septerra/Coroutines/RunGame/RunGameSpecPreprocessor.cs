using System;
using System.IO;

namespace Septerra
{
    public sealed class RunGameSpecPreprocessor : RunGameSpec
    {
        public String GameDirectoryPath { get; set; }

        public void Preprocess()
        {
            if (GameDirectory == null)
            {
                GameDirectory = new GameDirectoryDescriptor(Path.GetFullPath(GameDirectoryPath));
                if (!GameDirectory.IsMftExists)
                    throw new FileNotFoundException($"Cannot find a game archive file descriptor ({GameDirectory.MftPath}).");
            }
        }
    }
}