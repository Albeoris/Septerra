using System;
using System.Diagnostics;
using System.IO;
using Septerra.Core;

namespace Septerra.Core.Hooks
{
    public sealed class InteractionService : IService
    {
        public readonly String ExecutablePath = Asserts.FileExists(Path.GetFullPath("septerra.exe"));
        public readonly String DataDirectoryPath = Asserts.DirectoryExists(Path.GetFullPath("Data"));

        public InteractionService()
        {
            TXEncoding.TryReadFromExecutable(ExecutablePath);
        }
    }
}