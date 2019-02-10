using System;

namespace Septerra.Core
{
    public sealed class DbPackage
    {
        public Int32 Index { get; }
        public String Name { get; }
        public String FullPath { get; }

        public DbPackage(Int32 index, String name, String fullPath)
        {
            Index = index;
            Name = name;
            FullPath = fullPath;
        }
    }
}