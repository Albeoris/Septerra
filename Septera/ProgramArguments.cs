using System;
using System.IO;

namespace Septera
{
    public sealed class ProgramArguments
    {
        private readonly String[] _args;

        public ProgramArguments(String[] args)
        {
            _args = args ?? new String[0];
        }

        public String GetGameDirectoryPath()
        {
            if (_args.Length == 0)
                return CheckGameDirectoryPath(Environment.CurrentDirectory);

            return CheckGameDirectoryPath(_args[0]);
        }

        public String GetOutputDirectoryPath()
        {
            if (_args.Length < 2)
                return Path.Combine(GetGameDirectoryPath(), "Output");

            return Path.GetFullPath(_args[1]);
        }

        private String CheckGameDirectoryPath(String directoryPath)
        {
            String mftFilePath = Path.Combine(directoryPath, MftReader.MftFileName);
            if (!File.Exists(mftFilePath))
                throw new FileNotFoundException($"File [{mftFilePath}] not found. Set correct game directory path to the first command line argument or run this program from the game directory without arguments.");

            return directoryPath;
        }
    }
}