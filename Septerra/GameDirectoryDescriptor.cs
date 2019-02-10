using System;
using System.IO;

namespace Septerra
{
    public sealed class GameDirectoryDescriptor
    {
        public String DirectoryPath { get; }

        public GameDirectoryDescriptor(String gameDirectory)
        {
            DirectoryPath = gameDirectory;
        }

        public String ExecutablePath => Path.Combine(DirectoryPath, "septerra.exe");
        public String MftPath => Path.Combine(DirectoryPath, "septerra.mft");
        public String DataPath => Path.Combine(DirectoryPath, "Data");
        public Boolean IsExecutableExists => File.Exists(ExecutablePath);
        public Boolean IsMftExists => File.Exists(MftPath);
        public Boolean IsDataExists => Directory.Exists(DataPath);
    }
}